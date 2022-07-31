using System;
using UnityEngine;

namespace Luuklsl.Scripts.VibrationScripts
{
    /// <summary>
    /// Simple class which makes the HandGroup objects assignable in slightly more detail (object and channel per object)
    /// </summary>
    [Serializable]
    public class TrackedHandObjects
    {
        public GameObject trackedObject;

        [Range(1, 4)] public int channel;

        [NonSerialized] public float centerColliderDifference;
    }
}
