using System;
using UI;
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
            
            
            Manager.Inputs.OnLeftClick.performed += PatCheck;
        }

        private void PatCheck(InputAction.CallbackContext obj)
        {
            Ray ray = _cam.ScreenPointToRay(Manager.Inputs.MousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit) || hit.collider != _collider) return;
            OnDogPet?.Invoke();
            _particleSystem.Play();
            Manager.Jukebox.PlayBark();
            Notification.OnNotification.Invoke("You pet a dog!", icon, 0);
        }
    
        private void OnDestroy()
        {
            Manager.Inputs.OnLeftClick.performed -= PatCheck;
        }
    }
}