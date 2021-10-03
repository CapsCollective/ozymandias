using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace WalkingAdventurers
{
    public class Waterfall : MonoBehaviour
    {
        public Transform left, right;
        private Collider _collider;
        private Camera _cam;
        private bool _open;
        private void Start()
        {
            _cam = Camera.main;
            _collider = GetComponent<Collider>();
            
            Manager.Inputs.OnLeftClick.performed += _ =>
            {
                Ray ray = _cam.ScreenPointToRay(Manager.Inputs.MousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit) || hit.collider != _collider) return;
                Separate();
            };
        }

        [Button("Separate")]
        private void Separate()
        {
            if (_open) return;
            _open = true;
            
            left.DOLocalMoveX(1f, 1f);
            right.DOLocalMoveX(-1f, 1f);

            StartCoroutine(Algorithms.DelayCall(2f, () => {
                left.DOLocalMoveX(0, 1f);
                right.DOLocalMoveX(0, 1f);
                _open = false;
            }));
        }
    }
}