using System;
using UnityEngine;

namespace Luuklsl.Scripts.VibrationScripts
{
    public class ComplexVibrations : ContinuousStrengthPattern
    {
        [Range(0.0000001f, 20)] public float vibrationFrequency;

        private int vibrationStrength;

        protected override void OnCollisionStay(Collision other)
        {
            foreach (TrackedHandObjects otherObject in otherObjects)
            {
                if (other.gameObject == otherObject.trackedObject)
                {
                    //setup a sine-wave in which we can change the time of one period
                    vibrationStrength =
                        Math.Abs((int)Math.Round(middleStrength * (Mathf.Sin(vibrationFrequency * Time.time))));
                    // Debug.Log(Time.time);
                    // Debug.Log(vibrationStrength);
                    UpdateMessage(otherObject.channel, vibrationStrength);
                }
            }
        }
    }
}
