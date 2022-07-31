using UnityEngine;

namespace Luuklsl.Scripts
{
    public class FollowTargetObjectSmooth : MonoBehaviour
    {
        public Transform targetObject;
        public float strength = 1;
        public Vector3 offSet;

        // Update is called once per frame
        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, targetObject.position + offSet,
                strength * Time.deltaTime);
        }
    }
}
