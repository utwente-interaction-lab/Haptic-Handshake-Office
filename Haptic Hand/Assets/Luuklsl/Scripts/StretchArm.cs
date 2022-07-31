using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Luuklsl.Scripts
{
    public class StretchArm : MonoBehaviour
    {
        public TwoBoneIKConstraint IK_Constraint;

        [Range(0.01f, 2.0f)] public float speed = 0.4f;

        [SerializeField] private Animator animator;

        private bool anim;

        private float startTime;

        // Start is called before the first frame update
        void Start()
        {
            anim = false;
            // time = Time.time;
            if (IK_Constraint == null)
            {
                IK_Constraint = gameObject.GetComponent<TwoBoneIKConstraint>();
            }

            IK_Constraint.data.targetPositionWeight = 0;
            IK_Constraint.data.targetRotationWeight = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.H) && !anim)
            {
                anim = true;
                startTime = Time.time;
                animator.Play("HumanoidIdle");
            }
            else if (Input.GetKeyUp(KeyCode.H) && anim)
            {
                anim = false;
                startTime = Time.time;
                animator.Play("Locomotion");
            }

            if (!anim)
            {
                if (IK_Constraint.data.targetPositionWeight != 0)
                {
                    IK_Constraint.data.targetPositionWeight =
                        Math.Max(0, IK_Constraint.data.targetPositionWeight - 0.5f * speed);
                }

                if (IK_Constraint.data.targetRotationWeight != 0)
                {
                    IK_Constraint.data.targetRotationWeight =
                        Math.Max(0, IK_Constraint.data.targetRotationWeight - 0.5f * speed);
                }
            }

            if (anim)
            {
                IK_Constraint.data.targetPositionWeight = Math.Min(1 * (Time.time - startTime) / speed / 1.2f, 1);
                IK_Constraint.data.targetRotationWeight = Math.Min(1 * (Time.time - startTime) / speed, 1);
            }
        }

        public void Toggle()
        {
            anim = !anim;
            startTime = Time.time;
        }

        public void Set(bool state)
        {
            anim = state;
            startTime = Time.time;
        }
    }
}
