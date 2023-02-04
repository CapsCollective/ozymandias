using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Utilities
{
    public enum TaskState
    {
        Init,
        Running,
        Done
    }
    
    public class Task : IEnumerator
    {
        private static readonly object JumpToUnity = new object();
        private static readonly object JumpBack = new object();
        
        public static Task StartCoroutineAsync(MonoBehaviour script, IEnumerator routine)
        {
            Task task = new Task(routine);
            script.StartCoroutine(task);
            return task;
        }

        // implements IEnumerator to make it usable by StartCoroutine;
        #region IEnumerator Interface
        /// <summary>
        /// The current iterator yield return value.
        /// </summary>
        public object Current { get; private set; }

        /// <summary>
        /// Runs next iteration.
        /// </summary>
        /// <returns><code>true</code> for continue, otherwise <code>false</code>.</returns>
        public bool MoveNext()
        {
            return OnMoveNext();
        }

        public void Reset()
        {
            // Reset method not supported by iterator;
            throw new NotSupportedException(
                "Not support calling Reset() on iterator.");
        }
        #endregion

        // inner running state used by state machine;
        private enum RunningState
        {
            Init,
            RunningAsync,
            PendingYield,
            ToBackground,
            RunningSync,
            Done
        }

        // routine user want to run;
        private readonly IEnumerator _innerRoutine;

        // current running state;
        private RunningState _state;
        // last running state;
        private RunningState _previousState;
        // temporary stores current yield return value
        // until we think Unity coroutine engine is OK to get it;
        private object _pendingCurrent;
        
        private TaskState State
        {
            get
            {
                return _state switch
                {
                    RunningState.Done => TaskState.Done,
                    RunningState.Init => TaskState.Init,
                    _ => TaskState.Running
                };
            }
        }

        public Task(IEnumerator routine)
        {
            _innerRoutine = routine;
            // runs into background first;
            _state = RunningState.Init;
        }

        /// <summary>
        /// A co-routine that waits the task.
        /// </summary>
        public IEnumerator Wait()
        {
            while (State == TaskState.Running)
                yield return null;
        }

        // thread safely switch running state;
        private void GotoState(RunningState state)
        {
            if (_state == state) return;

            lock (this)
            {
                // maintainance the previous state;
                _previousState = _state;
                _state = state;
            }
        }

        // thread safely save yield returned value;
        private void SetPendingCurrentObject(object current)
        {
            lock (this)
            {
                _pendingCurrent = current;
            }
        }

        // actual MoveNext method, controls running state;
        private bool OnMoveNext()
        {
            // no running for null;
            if (_innerRoutine == null)
                return false;

            // set current to null so that Unity not get same yield value twice;
            Current = null;

            // loops until the inner routine yield something to Unity;
            while (true)
            {
                // a simple state machine;
                switch (_state)
                {
                    // first, goto background;
                    case RunningState.Init:
                        GotoState(RunningState.ToBackground);
                        break;
                    // running in background, wait a frame;
                    case RunningState.RunningAsync:
                        return true;

                    // runs on main thread;
                    case RunningState.RunningSync:
                        MoveNextUnity();
                        break;

                    // need switch to background;
                    case RunningState.ToBackground:
                        GotoState(RunningState.RunningAsync);
                        // call the thread launcher;
                        MoveNextAsync();
                        return true;

                    // something was yield returned;
                    case RunningState.PendingYield:
                        if (_pendingCurrent == JumpBack)
                        {
                            // do not break the loop, switch to background;
                            GotoState(RunningState.ToBackground);
                        }
                        else if (_pendingCurrent == JumpToUnity)
                        {
                            // do not break the loop, switch to main thread;
                            GotoState(RunningState.RunningSync);
                        }
                        else
                        {
                            // not from the Ninja, then Unity should get noticed,
                            // Set to Current property to achieve this;
                            Current = _pendingCurrent;

                            // yield from background thread, or main thread?
                            if (_previousState == RunningState.RunningAsync)
                            {
                                // if from background thread, 
                                // go back into background in the next loop;
                                _pendingCurrent = JumpBack;
                            }
                            else
                            {
                                // otherwise go back to main thread the next loop;
                                _pendingCurrent = JumpToUnity;
                            }

                            // end this iteration and Unity get noticed;
                            return true;
                        }
                        break;

                    // done running, pass false to Unity;
                    default:
                        return false;
                }
            }
        }

        // background thread launcher;
        private void MoveNextAsync()
        {
            ThreadPool.QueueUserWorkItem(BackgroundRunner);
        }

        // background thread function;
        private void BackgroundRunner(object state)
        {
            // just run the sync version on background thread;
            MoveNextUnity();
        }

        // run next iteration on main thread;
        private void MoveNextUnity()
        {
            try
            {
                // run next part of the user routine;
                var result = _innerRoutine.MoveNext();

                if (result)
                {
                    // something has been yield returned, handle it;
                    SetPendingCurrentObject(_innerRoutine.Current);
                    GotoState(RunningState.PendingYield);
                }
                else
                {
                    // user routine simple done;
                    GotoState(RunningState.Done);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
