using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using static Managers.GameManager;

namespace Characters
{
    public class Dog : MonoBehaviour
    {
        public static Action OnDogPet;
        
        [SerializeField] private Sprite icon;
        
        private Collider _collider;
        private ParticleSystem _particleSystem;

        private Camera _cam;
        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<Collider>();
            _particleSystem = GetComponent<ParticleSystem>();
            _cam = Camera.main;

            Manager.Inputs.LeftClick.performed += PatCheck;
        }

        private void PatCheck(InputAction.CallbackContext obj)
        {
            var hit = Manager.Inputs.GetRaycast(_cam, 1000, 1);
            if (hit.collider != _collider) return;
            OnDogPet?.Invoke();
            _particleSystem.Play();
            Manager.Jukebox.PlayBark();
            Manager.Notifications.Display("You pet a dog!", icon);
        }
    
        private void OnDestroy()
        {
            if (Globals.RestartingGame) return;
            Manager.Inputs.LeftClick.performed -= PatCheck;
        }
    }
}
