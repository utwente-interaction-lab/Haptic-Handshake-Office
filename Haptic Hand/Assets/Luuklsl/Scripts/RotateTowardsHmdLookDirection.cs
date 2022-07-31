using UnityEngine;

namespace Luuklsl.Scripts
{
    public class RotateTowardsHmdLookDirection : MonoBehaviour
    {
        [SerializeField] [Range(0.1f, 8)] private float strength;

        public bool strict;
        public bool rotating;

        [SerializeField] protected GameObject trackedCamera;

        [SerializeField] private float stopRotation;


        // Update is called once per frame
        void Update()
        {
            if (rotating)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    strict = !strict;
                    Debug.Log("Strict: " + strict);
                }

                if (strict)
                {
                    Helpers.RotateTowardsObject(trackedCamera.transform, this.transform, 90, strength, 1);
                }
                else
                {
                    Helpers.RotateTowardsObject(trackedCamera.transform, this.transform, 90, strength, stopRotation);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                rotating = !rotating;
                Debug.Log("Chair rotating");
            }
        }
    }
}
