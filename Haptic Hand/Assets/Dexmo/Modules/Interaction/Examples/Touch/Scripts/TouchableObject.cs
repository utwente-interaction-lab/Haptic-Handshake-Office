using Dexmo.Unity;
using UnityEngine;
using Dexmo.Unity.Interaction;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Simple touchable object instance
/// </summary>
public class TouchableObject : MonoBehaviour, ITouchable
{
    [SerializeField, HideInInspector]
    private bool forceFeedback = true;

    /// <summary>
    /// Stiffness of touchable object
    /// </summary>
    [SerializeField, HideInInspector]
    protected float stiffness = 1;

    /// <summary>
    /// Constrain finger model posture during touching status
    /// </summary>
    [SerializeField, HideInInspector]
    private bool constraintOnTouching = true;

    [SerializeField, HideInInspector]
    private bool updateTouchingPosition = true;

    [SerializeField, HideInInspector]
    private bool preventShakeOnExtremePosition = true;

    [SerializeField, HideInInspector]
    private SurfaceFinderMode surfaceFinderMode = SurfaceFinderMode.Inward;

    [SerializeField, HideInInspector]
    private float forceFeedbackDataFilterValue = 0f;

    [SerializeField, HideInInspector]
    private TouchUnityEvent onTouchEnter = new TouchUnityEvent();
    [SerializeField, HideInInspector]
    private TouchUnityEvent onTouchStay = new TouchUnityEvent();
    [SerializeField, HideInInspector]
    private TouchUnityEvent onTouchExit = new TouchUnityEvent();

    [SerializeField, HideInInspector]
    private bool vibrationFeedback = false;

    [SerializeField, HideInInspector]
    private TouchObjType vibrationObjType = TouchObjType.Impenetrable;

    [SerializeField, HideInInspector]
    private TouchObjSurface surfaceType = TouchObjSurface.Smooth;

    [SerializeField, HideInInspector]
    private float vibrationIntensity = 0.5f;

    [SerializeField, HideInInspector]
    private float limitRotateBendValueOnSurfaceFinding = 0.4f;

    private List<FingerType> touchingFingers = new List<FingerType>();

    public bool EnableForceFeedback { get { return forceFeedback; } set { forceFeedback = value; } }

    public Transform Transform { get { return transform; } }

    // public int LimitAngleOnTouching { get { return limitAngleOnTouching; } }

    public bool IsTouched { get; set; }
    public float Stiffness { get { return stiffness; } set { stiffness = Mathf.Clamp01(value); } }
    public bool EnableVibrationFeedback { get { return vibrationFeedback; } set { vibrationFeedback = value; } }

    public bool ConstraintOnTouching { get { return constraintOnTouching; } set { constraintOnTouching = value; } }

    public bool PreventShakeOnExtremePosition { get { return preventShakeOnExtremePosition; } set { preventShakeOnExtremePosition = value; } }

    public TouchObjType TouchObjType
    { get { return vibrationObjType; } set { vibrationObjType = value; } }
    public TouchObjSurface SurfaceType { get { return surfaceType; } set { surfaceType = value; } }
    public float VibrationIntensity { get { return vibrationIntensity; } set { vibrationIntensity = value; } }

    public TouchUnityEvent OnTouchEnter { get { return onTouchEnter; } }
    public TouchUnityEvent OnTouchStay { get { return onTouchStay; } }
    public TouchUnityEvent OnTouchExit { get { return onTouchExit; } }

    public SurfaceFinderMode FindSurfaceMode { get { return surfaceFinderMode; } set { surfaceFinderMode = value; } }

    public List<FingerType> TouchingFingers { get { return touchingFingers; } }

    public float LimitRotateBendValueOnSurfaceFinding { get { return limitRotateBendValueOnSurfaceFinding; } set { limitRotateBendValueOnSurfaceFinding = value; } }

    public float ForceFeedbackDataFilterValue { get { return forceFeedbackDataFilterValue; } set { forceFeedbackDataFilterValue = value; } }

    public bool UpdateTouchingPosition { get { return updateTouchingPosition; } set { updateTouchingPosition = value; } }
}