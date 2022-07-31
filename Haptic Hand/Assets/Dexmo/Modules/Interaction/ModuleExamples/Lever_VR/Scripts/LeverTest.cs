using Dexmo.Unity.Interaction;
using UnityEngine;

public class LeverTest : LeverForDexmo
{
    public Transform target;

	protected override void Start ()
    {
        base.Start();
        //SetFollowObj(target);
	}
	
	void Update ()
    {
        Rotate();	
	}
}
