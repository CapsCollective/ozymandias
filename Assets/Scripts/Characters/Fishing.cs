using System;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Fishing : MonoBehaviour
    {
        public static Action OnFishCaught;
        
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject fisher;
        [SerializeField] private ParticleSystem particles, splashParticles;
        private Collider _collider;
        private Camera _cam;
        private bool _fishing, _fishCaught;
        private MaterialPropertyBlock _materialProperty;
        private static readonly int CharacterDither = Shader.PropertyToID("_CharacterDither");

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _cam = Camera.main;

            Random.InitState(GetInstanceID());
            State.OnNextTurnEnd += () =>
            {
                bool active = Random.Range(0, 5) == 0;
                gameObject.SetActive(active);
            };
            Manager.Inputs.OnLeftClick.performed += FishCheck;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _materialProperty = new MaterialPropertyBlock();
            particles.Play(false);
        }

        private void FishCheck(InputAction.CallbackContext obj)
        {
            if (_fishing)
            {
                if (_fishCaught)
                {
                    OnFishCaught?.Invoke();
                    Notification.OnNotification.Invoke("You caught a fish!", icon, 0);
                    Manager.Stats.Wealth += 1;
                    UpdateUi();
                }
                Disable();
            }
            else
            {
                var hit = Manager.Inputs.GetRaycast(_cam, 1000, 1);
                if (hit.collider != _collider) return;

                _fishing = true;
                _fishCaught = false;
                particles.Stop();
                fisher.SetActive(true);
                _materialProperty.SetFloat(CharacterDither, 1);
                var body = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in body) r.SetPropertyBlock(_materialProperty);
                StartCoroutine(Algorithms.DelayCall(Random.Range(0.6f, 3f), FishCaught));
            }
        }

        private void FishCaught()
        {
            splashParticles.Play();
            Manager.Jukebox.PlayFishCaught();
            _fishCaught = true;
            StartCoroutine(Algorithms.DelayCall(2f, Disable));
        }

        private void Disable()
        {
            _fishing = false;
            _fishCaught = false;
            splashParticles.Stop();
            fisher.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
