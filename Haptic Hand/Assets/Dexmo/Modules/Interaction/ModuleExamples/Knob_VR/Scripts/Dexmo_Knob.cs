using System;
using UnityEngine;
using Dexmo.Unity;
using Dexmo.Unity.Interaction;

public class Dexmo_Knob : MonoBehaviour
{
    [SerializeField]
    private bool enable = true;

    [SerializeField]
    private Transform meshTransformOfKnob;

    [SerializeField]

    private SpecialMotionSettings knobRotateSettings;

    private float knobRotatedAngle = 0;

    private Vector3 knobRotateAxisVec3;

    private TouchableObject touchableObject;

    private ConstrainableObject constrainObject;

    private bool knobCanRotate = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        CaculateRotateAxisVec3();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        touchableObject = transform.GetComponentInChildren<TouchableObject>();
        constrainObject = transform.GetComponentInChildren<ConstrainableObject>();
        if (touchableObject == null || constrainObject == null)
        {
            Debug.unityLogger.LogError("Dexmo Knob", "Can not find touchable or constrainable object, knob module can not work!");
            enable = false;
        }
        else
        {
            touchableObject.OnTouchStay.AddListener(DetectTouchStatus);
            constrainObject.OnConstrainStay.AddListener(ListenConstraintRotateAngle);
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if (touchableObject != null)
            touchableObject.OnTouchStay.RemoveListener(DetectTouchStatus);
        if (constrainObject != null)
            constrainObject.OnConstrainStay.RemoveListener(ListenConstraintRotateAngle);
    }

    #region Listen to touchable object event

    private void DetectTouchStatus(TouchInteractionData _touchData)
    {
        if (touchableObject.TouchingFingers.Contains(FingerType.THUMB) && touchableObject.TouchingFingers.Count >= 2)
        {
            if (!knobCanRotate)
            {
                knobCanRotate = true;
                // Debug.Log("Enable constraint");
                var _locationConstraintData = constrainObject.GetHandLocationData(_touchData.Handedness);
                if (_locationConstraintData.EnableConstrain)
                {
                    _locationConstraintData.ConstrainRotation.IgnoreAxisSettings.AxisMotionSettings = SpecialAxisMotionSettings.Free;
                    _locationConstraintData.ConstrainRotation.RotatePositionReferenceAxisSettings.AxisMotionSettings = SpecialAxisMotionSettings.Free;
                }
            }
        }
        else
        {
            if (knobCanRotate)
            {
                knobCanRotate = false;
                // Debug.Log("DisEnable constraint");
                var _locationConstraintData = constrainObject.GetHandLocationData(_touchData.Handedness);
                if (_locationConstraintData.EnableConstrain)
                {
                    _locationConstraintData.ConstrainRotation.IgnoreAxisSettings.AxisMotionSettings = SpecialAxisMotionSettings.None;
                    _locationConstraintData.ConstrainRotation.RotatePositionReferenceAxisSettings.AxisMotionSettings = SpecialAxisMotionSettings.None;
                }
            }
        }
    }

    #endregion

    #region  Listen to constrainable object event

    private void ListenConstraintRotateAngle(ConstraintInteractionData _constraintData)
    {
        float _offsetValue = _constraintData.SelfRotateAngle;
        if (knobCanRotate && knobRotateSettings.CheckSpecialMotionValueIlegal(_offsetValue, knobRotatedAngle))
        {
            knobRotatedAngle += _offsetValue;
            RotateKnobMesh(_offsetValue);
        }
    }

    #endregion

    private void CaculateRotateAxisVec3()
    {
        switch (knobRotateSettings.Axis)
        {
            case SpecialAxis.X:
                knobRotateAxisVec3 = Vector3.right;
                break;
            case SpecialAxis.Y:
                knobRotateAxisVec3 = Vector3.up;
                break;
            case SpecialAxis.Z:
                knobRotateAxisVec3 = Vector3.forward;
                break;
            case SpecialAxis.None:
                knobRotateAxisVec3 = Vector3.zero;
                break;
        }
    }

    private void RotateKnobMesh(float _rotateAngle)
    {
        if (enable)
        {
            if (meshTransformOfKnob != null)
                meshTransformOfKnob.Rotate(knobRotateAxisVec3, _rotateAngle);
        }
    }
}
