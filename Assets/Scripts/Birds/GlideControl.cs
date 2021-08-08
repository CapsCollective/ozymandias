using UnityEngine;

namespace Birds
{
    public class GlideControl : MonoBehaviour
    {
        private Animator myAnim;
        public BoidSettings settings;
        private float flapAngle;

        private void Awake()
        {
            myAnim = GetComponent<Animator>();
            flapAngle = settings.flapAngle;
        }

        // Update is called once per frame
        void Update()
        {
            if(flapAngle!= settings.flapAngle)
            {
                flapAngle = settings.flapAngle;
            }
            myAnim.SetBool("isGlide", Mathf.Abs(transform.rotation.eulerAngles.x) < flapAngle);
        }
    }
}
