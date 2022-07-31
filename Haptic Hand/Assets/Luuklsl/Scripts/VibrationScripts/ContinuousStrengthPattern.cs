using System.Linq;
using UnityEngine;

namespace Luuklsl.Scripts.VibrationScripts
{
    public class ContinuousStrengthPattern : BaseVibrationScript
    {
        [Header("Booleans for controlling vibration parts")] [Tooltip("Do you want a continuous high signal on collision?")]
        public bool continuous;

        [Tooltip("If continuous is off, do you want vibration in the Middle??")]
        public bool middleOn;

        [Tooltip("If off, you get no initial peak")]
        public bool firstPeak;

        [Tooltip("If off, get no second peak")]
        public bool secondPeak;

        [Header("Vibration Strengths")]
        [Tooltip("Strength of Vibration in the Middle, only used when middleON is checked")]
        [Range(0, 255)]
        public int middleStrength;

        [Tooltip("Vibration Strength of beginning and end-peak")] [Range(0, 255)]
        public int maxStrength;

        [Header("Peak width in amount of frames")]
        [Tooltip("How long do you want to have a vibration? 60 frames is one second")]
        [Range(5, 100)]
        public int maxStrengthAmountOfFrames;


        /*
     * Both the Start() and Update() are called in the base class
     * 
     */
        protected override void Start()
        {
            base.Start();
            Debug.Log("ContinuousStrengthPattern Started");
        }


        private void OnCollisionEnter(Collision other)
        {
            foreach (TrackedHandObjects handObjects in otherObjects)
            {
                if (other.gameObject == handObjects.trackedObject && firstPeak && channelFree[handObjects.channel - 1])
                {
                    PeakResponse(handObjects.channel);
                }
            }
        }

        protected virtual void OnCollisionStay(Collision other)
        {
            // Debug.Log(other.collider);
            foreach (TrackedHandObjects handObjects in otherObjects)
            {
                if (other.gameObject == handObjects.trackedObject)
                {
                    if (continuous)
                    {
                        // Strength.Enqueue(maxStrength);
                        UpdateMessage(handObjects.channel, maxStrength);
                        //keep at max
                    }
                    else if (!continuous && middleOn)
                    {
                        // Strength.Enqueue(middleStrength);
                        UpdateMessage(handObjects.channel, middleStrength);
                    }
                    else if (!continuous && !middleOn)
                    {
                        // Strength.Enqueue(0);
                        UpdateMessage(handObjects.channel, 0);
                    }
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            Debug.Log(other.collider);
            foreach (TrackedHandObjects handObjects in otherObjects)
            {
                if (other.gameObject == handObjects.trackedObject && secondPeak && channelFree[handObjects.channel - 1])
                {
                    PeakResponse(handObjects.channel);
                }
            }
        }

        protected void PeakResponse(int channel)
        {
            channelFree[channel - 1] = false;
            foreach (int loops in Enumerable.Range(0, maxStrengthAmountOfFrames))
            {
                switch (channel)
                {
                    case 1:
                        Channel1Strength.Enqueue(maxStrength);
                        break;
                    case 2:
                        Channel2Strength.Enqueue(maxStrength);
                        break;
                    case 3:
                        Channel3Strength.Enqueue(maxStrength);
                        break;
                    case 4:
                        Channel4Strength.Enqueue(maxStrength);
                        break;
                    default:
                        Debug.Log("Something went wrong here, I shouldn't ever be shown");
                        break;
                }
            }

            switch (channel)
            {
                case 1:
                    Channel1Strength.Enqueue(0);
                    break;
                case 2:
                    Channel2Strength.Enqueue(0);
                    break;
                case 3:
                    Channel3Strength.Enqueue(0);
                    break;
                case 4:
                    Channel4Strength.Enqueue(0);
                    break;
            }
        }
    }
}
