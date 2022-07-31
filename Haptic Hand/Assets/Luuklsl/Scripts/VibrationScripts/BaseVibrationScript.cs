using System.Collections.Generic;
using UnityEngine;

namespace Luuklsl.Scripts.VibrationScripts
{
    public class BaseVibrationScript : MonoBehaviour
    {
        /// The object to check collisions or distance with.
        [Header("Referenced Objects")] // ignore this line, it only generates a header in Unity
        [Tooltip("What object do we want to watch for?")]
        // public GameObject otherObject;
        public TrackedHandObjects[] otherObjects;

        /// The object which handles the setting up of networking and sending of messages with the Feather
        [Tooltip("The Network object which sends and receives data")]
        public GameObject networkObject;


        /// strength is the queue of messages to be sent to the vibrotactile glove
        /// TODO: Check if this is possible with a list of Queues?
        protected Queue<int> Channel1Strength = new Queue<int>();

        protected Queue<int> Channel2Strength = new Queue<int>();
        protected Queue<int> Channel3Strength = new Queue<int>();
        protected Queue<int> Channel4Strength = new Queue<int>();

        protected List<bool> channelFree = new List<bool>() { true, true, true, true };

        protected ServerClient script;

        private Dictionary<string, int> message = new Dictionary<string, int>();
        private Dictionary<string, int> oldMessage = new Dictionary<string, int>();

        protected virtual void Start()
        {
        Debug.Log("BaseVibrationscript Started");
            // Debug.Log(networkObject);
            if (networkObject == null)
            {
                networkObject = GameObject.Find("Network");
            }

            script = networkObject.GetComponent<ServerClient>();

            if (script == null)
            {
                Debug.LogError("Script wasn't found, you sure you connected the right object?");
                // UnityEditor.EditorApplication.isPlaying = false;
            }

            message.Add("m1Intensity", 0);
            message.Add("m2Intensity", 0);
            message.Add("m3Intensity", 0);
            message.Add("m4Intensity", 0);

            oldMessage.Add("m1Intensity", 0);
            oldMessage.Add("m2Intensity", 0);
            oldMessage.Add("m3Intensity", 0);
            oldMessage.Add("m4Intensity", 0);
        }

        protected virtual void Update()
        {
            //Channel details:
            /* M1 = pin 13, M2 = pin 12
         M3 = pin 27, M4 = pin 33 */

            // Check if we have something in a queue, which would mean a peak, check for both channels. Dirty, but does the trick
            if (Channel1Strength.Count > 0)
            {
                int value = Channel1Strength.Dequeue();
                message["m1Intensity"] = value;
            }
            else
            {
                channelFree[0] = true;
            }

            if (Channel2Strength.Count > 0)
            {
                int value = Channel2Strength.Dequeue();
                message["m2Intensity"] = value;
            }
            else
            {
                channelFree[1] = true;
            }

            if (Channel3Strength.Count > 0)
            {
                int value = Channel3Strength.Dequeue();
                message["m3Intensity"] = value;
            }
            else
            {
                channelFree[2] = true;
            }

            if (Channel4Strength.Count > 0)
            {
                int value = Channel4Strength.Dequeue();
                message["m4Intensity"] = value;
            }
            else
            {
                channelFree[3] = true;
            }

            //Only send a message if we actually change, would be wasteful otherwise.


            if (!DictEquals(message, oldMessage))
            {
                script.SendServerMessage(message);
                oldMessage = new Dictionary<string, int>(message);
                // Debug.Log(JsonConvert.SerializeObject(oldMessage));
            }
        }

        private static bool DictEquals(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
        {
            // Test for equality.
            bool equal = false;
            if (dict1.Count == dict2.Count) // Require equal count.
            {
                equal = true;
                foreach (var pair in dict1)
                {
                    if (dict2.TryGetValue(pair.Key, out int value))
                    {
                        // Require value be equal.
                        if (value != pair.Value)
                        {
                            equal = false;
                            break;
                        }
                    }
                    else
                    {
                        // Require key be present.
                        equal = false;
                        break;
                    }
                }
            }

            return equal;
        }


        protected void UpdateMessage(int channel, int strength)
        {
            switch (channel)
            {
                case 1:
                    message["m1Intensity"] = strength;
                    break;
                case 2:
                    message["m2Intensity"] = strength;
                    break;
                case 3:
                    message["m3Intensity"] = strength;
                    break;
                case 4:
                    message["m4Intensity"] = strength;
                    break;
            }
        }
    }
}
