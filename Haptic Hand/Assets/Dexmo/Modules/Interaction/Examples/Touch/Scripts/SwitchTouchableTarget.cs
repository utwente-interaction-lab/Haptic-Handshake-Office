using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwitchTouchableTarget : MonoBehaviour
{

    [Serializable]
    public class TouchableTargetGroup
    {
        public GameObject Target_L;
        public GameObject Target_R;

        public void SetActive(bool _isActive)
        {
            Target_L.SetActive(_isActive);
            Target_R.SetActive(_isActive);
        }
    }

    [SerializeField]
    private TouchableTargetGroup[] touchableTargetGroups;

    private KeyCode switchKeyCode = KeyCode.C;

    private int curTouchableTargetGroupIndex = -1;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(switchKeyCode))
        {
            if (curTouchableTargetGroupIndex >= 0)
                touchableTargetGroups[curTouchableTargetGroupIndex].SetActive(false);
            curTouchableTargetGroupIndex++;
            if (curTouchableTargetGroupIndex >= touchableTargetGroups.Length)
                curTouchableTargetGroupIndex = 0;
            touchableTargetGroups[curTouchableTargetGroupIndex].SetActive(true);
        }
    }
}
