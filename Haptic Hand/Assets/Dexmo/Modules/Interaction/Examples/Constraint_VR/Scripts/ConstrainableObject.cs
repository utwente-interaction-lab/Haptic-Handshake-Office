using Dexmo.Unity;
using Dexmo.Unity.Interaction;
using UnityEngine;
using Dexmo.Unity.Model;

public class ConstrainableObject : MonoBehaviour, IConstrainable
{
    public bool PreviewPostureData;
    public UnityHandModel handModel_L;
    public UnityHandModel handModel_R;
    public ConstrainHandData LeftHandConstraintData = new ConstrainHandData();
    public ConstrainHandData RightHandConstraintData = new ConstrainHandData();

    [SerializeField]
    private ConstraintUnityEvent onConstraintEnter = new ConstraintUnityEvent();
    [SerializeField]
    private ConstraintUnityEvent onConstraintStay = new ConstraintUnityEvent();
    [SerializeField]
    private ConstraintUnityEvent onConstraintExit = new ConstraintUnityEvent();
    public Transform Transform { get { return transform; } }
    public bool IsConstrained { get; set; }

    public ConstraintUnityEvent OnConstrainEnter { get { return onConstraintEnter; } }

    public ConstraintUnityEvent OnConstrainStay { get { return onConstraintStay; } }

    public ConstraintUnityEvent OnConstrainExit { get { return onConstraintExit; } }

    public ConstrainHandLocationData GetHandLocationData(Handedness _handedness)
    {
        return _handedness == Handedness.Left ? LeftHandConstraintData.LocationData : RightHandConstraintData.LocationData;
    }

    public ConstrainHandPostureData GetHandPostureData(Handedness _handedness)
    {
        return _handedness == Handedness.Left ? LeftHandConstraintData.PostureData : RightHandConstraintData.PostureData;
    }


}
