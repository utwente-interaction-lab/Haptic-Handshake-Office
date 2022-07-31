using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Luuklsl.Scripts
{
    public class LookAtHeadOfParticipant : MonoBehaviour
    {
        private bool focusOnParticipant;
        private bool newFocus = true;

        public Transform targetObject;
        public float strength = 1;
        private MultiAimConstraint constraint;
        private float timeStep;
        private float lerpStrength;

        // Start is called before the first frame update
        void Start()
        {
            constraint = gameObject.GetComponentInParent<MultiAimConstraint>();
            if (constraint == null)
            {
                Debug.Log("No constraint object found");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.G))
            {
                focusOnParticipant = !focusOnParticipant;
                timeStep = 0;
                lerpStrength = 0;
                newFocus = true;
            }

            // Debug.Log(targetObject.position + " "+ gameObject.transform.position +" "+ 0.000000001 * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, targetObject.position,
                3 * Time.deltaTime);

            switch (focusOnParticipant)
            {
                case true when newFocus:
                {
                    lerpStrength = timeStep * Time.deltaTime * (1 / strength) + lerpStrength;
                    constraint.weight = Mathf.Lerp(0, 1, Mathf.Min(lerpStrength, 1));
                    if (lerpStrength <= 1)
                    {
                        timeStep += 0.1f;
                    }
                    else
                    {
                        newFocus = false;
                    }

                    // lerp weight to look at face;
                    break;
                }
                case false when newFocus:
                {
                    // opposite of above
                    lerpStrength = timeStep * Time.deltaTime * (1 / strength) + lerpStrength;
                    constraint.weight = Mathf.Lerp(1, 0, Mathf.Min(lerpStrength, 1));

                    if (lerpStrength <= 1)
                    {
                        timeStep += 0.1f;
                    }
                    else
                    {
                        newFocus = false;
                    }

                    break;
                }
            }
        }
    }
}
