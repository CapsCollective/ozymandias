using Achievements;
using Inputs;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Platform
{
    public enum PlatformID
    {
        Base,
        Steam,
        Switch
    }
    
    public class PlatformDelegate
    {
        protected virtual PlatformID GetPlatformId()
        {
            return PlatformID.Base;
        }
    }

    public class AchievementsDelegate : PlatformDelegate
    {
        public virtual void Initialise()
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.Initialise");
        }
        
        public virtual void UnlockAchievement(Achievement achievement)
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.UnlockAchievement");
        }
        
        public virtual void UpdateStat(GameStat stat, int value)
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.UpdateStat");
        }
        
        public virtual void UpdateProgress(GameStat stat, Achievement achievement, int value)
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.UpdateProgress");
        }
        
        public virtual void ResetAll()
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.ResetAll");
        }
    }
    
    public class InputDelegate : PlatformDelegate
    {
        public virtual InputControlScheme GetDefaultControlScheme(PlayerInputs playerInputs)
        {
            return playerInputs.controlSchemes[0];
        }
    }
    
    public class FileSystemDelegate : PlatformDelegate
    {
        public virtual string GetSaveFilePath()
        {
            return Application.persistentDataPath + "/save.json";
        }
    }
}