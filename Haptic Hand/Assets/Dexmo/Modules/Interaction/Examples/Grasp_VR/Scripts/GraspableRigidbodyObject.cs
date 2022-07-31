using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GraspableRigidbodyObject : GraspableObject
{
    private Rigidbody rb;
    
    private bool usedGravity;
    private bool isKinematic;

    public bool GravityReleased { get { return usedGravity; } set { usedGravity = value; } }
    public bool IsKinematicReleased { get { return isKinematic; } set { isKinematic = value; } }

    public override void Init(Transform _reference)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        usedGravity = rb.useGravity;
        isKinematic = rb.isKinematic;
        rb.useGravity = false;
        rb.isKinematic = true;
        base.Init(_reference);
    }
    
    public override void Release()
    {
        rb.useGravity = usedGravity;
        rb.isKinematic = isKinematic;
    }
}
