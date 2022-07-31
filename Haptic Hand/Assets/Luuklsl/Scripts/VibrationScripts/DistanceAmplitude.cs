using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Luuklsl.Scripts.VibrationScripts
{
    public class DistanceAmplitude : BaseVibrationScript
    {
        [Header("Vibration Strengths")] [Tooltip("Maximum vibration strength")] [Range(0, 255)]
        public int maxStrength;

        [Header("Trigger Distance")] [Tooltip("Distance from which the vibration should start")] [Range(0, 10)]
        public float maxDistance;

        private float selfObjectSize;
        private float otherObjectSize;
        private float distance;

        private int distanceStrength;

        //Build a list of x lists (x being the max range of channels) to store data in temp.
        //Chosen for lists as they are variable length, but useful collections (.Max in our case)
        List<List<int>> channelStrengths = new List<List<int>>();


        /// <summary>
        /// Partially overrides Start with some script specific calculations, then also calls base method
        /// </summary>
        protected override void Start()
        {
            channelStrengths.Add(new List<int>());
            channelStrengths.Add(new List<int>());
            channelStrengths.Add(new List<int>());
            channelStrengths.Add(new List<int>());

            //TODO: MAKE THIS WORK FOR THE FINAL ITTERATION. Hand colliders are different!

            selfObjectSize = GetComponent<MeshFilter>().sharedMesh.bounds.max.x * transform.localScale.x;
            foreach (TrackedHandObjects handObjects in otherObjects)
            {
                Collider otherObjectCollider = handObjects.trackedObject.GetComponent<Collider>();
                if (otherObjectCollider is CapsuleCollider)
                {
                    otherObjectSize = handObjects.trackedObject.GetComponent<CapsuleCollider>().radius *
                                      handObjects.trackedObject.transform.localScale.x;
                }
                else if (otherObjectCollider is BoxCollider)
                {
                    otherObjectSize = handObjects.trackedObject.GetComponent<BoxCollider>().size.x *
                                      handObjects.trackedObject.transform.localScale.x;
                }
                else if (otherObjectCollider is SphereCollider)
                {
                    otherObjectSize = handObjects.trackedObject.GetComponent<SphereCollider>().radius *
                                      handObjects.trackedObject.transform.localScale.x;
                }

                // otherObjectSize = handObjects.trackedObject.GetComponent<CapsuleCollider>().radius * 
                //                   handObjects.trackedObject.transform.localScale.x;
                handObjects.centerColliderDifference = selfObjectSize + otherObjectSize;
                // Debug.Log(handObjects.centerColliderDifference);
                // Debug.Log(handObjects.trackedObject.GetComponent<Collider>().ClosestPointOnBounds(this.transform.position).magnitude);
            }

            base.Start();
        }

        // Update is called once per frame, this one overrides super-class partially, to then call previous expected behaviour
        protected override void Update()
        {
            //Todo: make sure that we don't accidentally write a 0 when the last finger in the loop is disconnected from the object this script is on
            bool updated = false;


            foreach (TrackedHandObjects handObjects in otherObjects)
            {
                // Get distance between objects centers
                distance = Vector3.Distance(handObjects.trackedObject.transform.position, transform.position);

                //remove total pre-calculated collider difference 
                distance = distance - handObjects.centerColliderDifference;
                // Debug.Log(distance);

                // if distance is smaller than max distance, we want to send some information
                if (distance < maxDistance)
                {
                    // Calculate the Strength, inversely related to distance
                    distanceStrength = (int)Math.Round(maxStrength - (maxStrength * distance / maxDistance));
                    // Strength.Enqueue(distanceStrength);
                    // UpdateMessage(handObjects.channel, distanceStrength);
                    channelStrengths[handObjects.channel].Add(distanceStrength);
                    updated = true;
                }
            }

            if (updated)
            {
                for (int i = 1; i < 5; i++)
                {
                    try
                    {
                        UpdateMessage(i, channelStrengths[i].Max());
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        continue;
                    }
                }
            }
            else
            {
                //todo figure out why this doesn't always trigger
                for (int i = 1; i < 5; i++)
                {
                    UpdateMessage(i, 0);
                }
            }

            foreach (List<int> channel in channelStrengths)
            {
                channel.Clear();
            }

            base.Update();
        }
    }
}
