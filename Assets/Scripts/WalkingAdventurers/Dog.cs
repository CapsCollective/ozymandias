using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using static Managers.GameManager;

namespace WalkingAdventurers
{
    public class Dog : MonoBehaviour
    {
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
            _particleSystem.Play();
            Manager.Jukebox.PlayBark();
            Notification.OnNotification.Invoke("You pat a dog!", icon);
        }
    
        private void OnDestroy()
        {
            Manager.Inputs.OnLeftClick.performed -= PatCheck;
        }
    }
}
