using System.Collections.Generic;
using UnityEngine;

namespace Dexmo.Unity.Interaction
{
    public delegate void KnobHoldingEvent();

    public class KnobForDexmo_Dev : RotaryBase
    {
        public event KnobHoldingEvent holdingStart;
        public event KnobHoldingEvent holdingEnd;
        private bool _startFollow = false;

        protected bool isHolding = false;
        protected Transform _FollowObj;

        private Vector3 _InitProject;
        private ITouchable touchable;

        protected override void Start()
        {
            base.Start();
            rotaryType = RotaryType.WithSelfAxis;
            touchable = RotateTarget.GetComponentInChildren<ITouchable>();
            touchable.OnTouchStay.AddListener(OnInteractionStay);
        }

        public virtual void OnInteractionStay(TouchInteractionData _data)
        {
            var others = 0;
            var thumb = 0;
            foreach (FingerTouchInteractionData _fingerData in _data.FingerTouchInteractionDatas.Values)
            {
                if (_fingerData.TouchTarget == touchable && _fingerData.Status == TouchStatus.Touching)
                {
                    if (_fingerData.Type == FingerType.THUMB)
                        thumb++;
                    else
                    {
                        others++;
                    }
                }
            }

            if (thumb <= 0 || others <= 0) 
            {
                if (_startFollow == true)
                {
                    _startFollow = false;
                    if (holdingEnd != null)
                        holdingEnd.Invoke();
                }
                isHolding = false;
            }

            if (thumb > 0 && others > 0 && isHolding == false)
            {
                if (_startFollow == false)
                {
                    Debug.Log("StartFollowObj");
                    SetFollowObj(_data.HandModelAttached.PalmTrans);
                    _startFollow = true;
                    if (holdingStart != null)
                        holdingStart.Invoke();
                }
                isHolding = true;
            }

            Rotate();
        }

        public virtual void OnInteractionExit(TouchInteractionData _data)
        {
            _FollowObj = null;
            isHolding = false;
        }

        private void SetFollowObj(Transform obj)
        {
            if (obj == null) return;

            _FollowObj = obj;
            _InitProject = _GetPlaneProject(obj);

            lastRotateAngle = 0;
            tempAngle = 0;
        }

        protected override bool CanRotate()
        {
            return _FollowObj != null && isHolding == true;
        }

        protected override float GetRotateAngle()
        {
            return AngleTool.GetAngle(_InitProject, _GetPlaneProject(_FollowObj), 
                RotateReference.TransformDirection(rotateAxis), true);
        }

        private Vector3 _GetPlaneProject(Transform target)
        {
            return Vector3.ProjectOnPlane(target.forward,
                RotateReference.TransformDirection(rotateAxis));
        }
    }
}