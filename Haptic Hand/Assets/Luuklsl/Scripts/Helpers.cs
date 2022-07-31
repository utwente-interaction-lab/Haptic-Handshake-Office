using System;
using UnityEngine;

namespace Luuklsl.Scripts
{
    public class Helpers : MonoBehaviour
    {
        private static float _lerpStrength;

        public static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            float num1 = a.x - b.x;
            float num2 = a.z - b.z;
            return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2);
        }


        public static bool RotateTowardsObject(Transform focusObject, Transform turningObject, float offset = 0,
            float strength = 1f, float stopDeviation = 4f)
        {
            // ReSharper disable once Unity.InefficientPropertyAccess
            focusObject.position = new Vector3(focusObject.position.x, 0, focusObject.position.z);
            turningObject.position = new Vector3(turningObject.position.x, 0, turningObject.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(focusObject.position - turningObject.position);

            if (offset != 0)
            {
                targetRotation = Quaternion.Euler(0, offset, 0) * targetRotation;
            }

            if (Quaternion.Angle(targetRotation, turningObject.rotation) < stopDeviation)
            {
                return true;
            }

            _lerpStrength = Mathf.Min(strength * Time.deltaTime, 1);
            turningObject.rotation = Quaternion.Lerp(turningObject.rotation, targetRotation, _lerpStrength);
            return false;
        }
    }
}
