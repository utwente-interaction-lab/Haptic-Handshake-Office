using UnityEngine;
using Dexmo.Unity;
using Dexmo.Unity.Interaction;
using UnityEngine.Events;

/// <summary>
/// Simple graspable object
/// </summary>
public class GraspableObject : MonoBehaviour, IGraspable
{
    [SerializeField]
    private bool continuousSurfaceFinder = false;

    [SerializeField]
    private SpecialMotionSettings constraintSettings;

    /// <summary>
    /// Executed when object is enter grasped status
    /// </summary>
    [SerializeField]
    private GraspUnityEvent onGraspEnter = new GraspUnityEvent();

    /// <summary>
    /// Executed when object is stay in grasped status
    /// </summary>
    [SerializeField]
    private GraspUnityEvent onGraspStay = new GraspUnityEvent();

    /// <summary>
    /// Executed when object is exit from grasped status
    /// </summary>
    [SerializeField]
    private GraspUnityEvent onGraspExit = new GraspUnityEvent();

    /// <summary>
    /// Returns object's transform compoment 
    /// </summary>
    /// <value></value>
    public Transform Transform { get { return transform; } }

    public SpecialMotionSettings ConstraintMotionSettings { get { return constraintSettings; } }

    /// <summary>
    /// Returns if object is grasped
    /// </summary>
    /// <value></value>
    public bool IsGrasped { get; set; }

    public bool ContinuousSurfaceFinder { get { return continuousSurfaceFinder; } set { continuousSurfaceFinder = value; } }

    /// <summary>
    /// The position offset for reference transform and graspable target transform
    /// </summary>
    private Vector3 positionOffset = Vector3.zero;

    /// <summary>
    /// The rotation offset for reference transform and graspable target transform
    /// </summary>
    private Quaternion rotationOffset;

    private Vector3 lastReferencePosition = Vector3.zero;

    protected float totalConstraintMoveDistance = 0f;

    private SpecialAxis curConstraintAxis = SpecialAxis.None;

    private Vector3 moveAxisVec3 = Vector3.zero;

    public virtual void Init(Transform graspReference)
    {
        curConstraintAxis = constraintSettings.Axis;
        lastReferencePosition = graspReference.position;
        switch (curConstraintAxis)
        {
            case SpecialAxis.X:
                moveAxisVec3 = Vector3.right;
                break;
            case SpecialAxis.Y:
                moveAxisVec3 = Vector3.up;
                break;
            case SpecialAxis.Z:
                moveAxisVec3 = Vector3.forward;
                break;
        }
        positionOffset = Quaternion.Inverse(graspReference.rotation) * (transform.position - graspReference.position);
        rotationOffset = Quaternion.Inverse(graspReference.rotation) * transform.rotation;
    }

    public virtual void Release()
    {
        positionOffset = Vector3.zero;
        rotationOffset = Quaternion.identity;
    }


    public virtual void Follow(Transform graspReference)
    {
        if (curConstraintAxis != constraintSettings.Axis)
        {
            Init(graspReference);
        }
        float _offsetDistance = 0;
        switch (curConstraintAxis)
        {
            case SpecialAxis.None:
                transform.position = graspReference.position + graspReference.rotation * positionOffset;
                transform.rotation = graspReference.rotation * rotationOffset;
                break;
            case SpecialAxis.X:
                _offsetDistance = lastReferencePosition.MoveDistanceOnNormalVector(graspReference.position, transform.right);// graspReference.lossyScale.x
                break;
            case SpecialAxis.Y:
                _offsetDistance = lastReferencePosition.MoveDistanceOnNormalVector(graspReference.position, transform.up);
                break;
            case SpecialAxis.Z:
                _offsetDistance = lastReferencePosition.MoveDistanceOnNormalVector(graspReference.position, transform.forward);
                break;
        }
        if (constraintSettings.CheckSpecialMotionValueIlegal(_offsetDistance, totalConstraintMoveDistance))
        {
            totalConstraintMoveDistance += _offsetDistance;
            transform.Translate(moveAxisVec3 * _offsetDistance);
        }
        lastReferencePosition = graspReference.position;
    }

    /// <summary>
    /// Returns grasp enter unity event instance
    /// </summary>
    /// <value></value>
    public GraspUnityEvent OnGraspEnter { get { return onGraspEnter; } }

    /// <summary>
    /// Returns grasp stay unity event instance
    /// </summary>
    /// <value></value>
    public GraspUnityEvent OnGraspStay { get { return onGraspStay; } }

    /// <summary>
    /// Returns grasp exit unity event instance
    /// </summary>
    /// <value></value>
    public GraspUnityEvent OnGraspExit { get { return onGraspExit; } }


}