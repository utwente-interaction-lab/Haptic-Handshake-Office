using UnityEngine;
using Valve.VR;

namespace Luuklsl.Scripts
{
    public class SetTrackedDeviceID : MonoBehaviour
    {
        public SteamVR_TrackedObject SteamVRTrackedObject;

        private void Awake()
        {
            SteamVRTrackedObject = gameObject.GetComponent<SteamVR_TrackedObject>();
            for (uint i = 0; i < 16; i++)
            {
                var result = OpenVR.System.GetTrackedDeviceClass(i);
                if (result == ETrackedDeviceClass.GenericTracker)
                {
                    SteamVRTrackedObject.SetDeviceIndex((int)i);
                    break;
                }
            }
        }
    }
}
