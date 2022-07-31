/*
 * @Author: Dexta Robotics
 * @Date: 2019-06-14 20:05:04
 * @LastEditors: CK
 * @LastEditTime: 2019-01-14 18:42:10
 * @Description: 
 * @Modify Description: &Modify Description&
 */
using UnityEngine;
using Dexmo.Unity.Motion;
using System.Collections.Generic;

namespace Dexmo.Unity.Interaction
{
    public class LeverForDexmo : RotaryBase
    {
        public event RotaryAngleChange AngleChange;
        public event RotaryAngleChange LeverOpenStart;
        public event RotaryAngleChange LeverOpenEnd;
        public event RotaryAngleChange LeverCloseStart;
        public event RotaryAngleChange LeverCloseEnd;

        [SerializeField]
        private float LeverOpenAngle = 10;
        [SerializeField]
        private float LeverCloseAngle = 165;

        [SerializeField]
        private Transform touchableObj;
        [SerializeField]
        private Transform LeftConstrainReference;
        [SerializeField]
        private Transform RightConstrainReference;

        protected bool isLeverEnable = true;
        protected bool isHolding = false;
        protected Transform followObj;
        protected ITouchable touchable;
        protected Handedness handedness;

        private LeverState _leverState;
        private IDexmoMotionControl _DexmoMotionCtrl;

        private Dictionary<Handedness, LeverConstrainData> _constrainDatas;
        private int _FingerCount = 4;

        protected override void Start()
        {
            base.Start();
            rotaryType = RotaryType.WithOtherAxis;
            _DexmoMotionCtrl = DexmoManager.Instance.DexmoMotionControl;

            _constrainDatas = new Dictionary<Handedness, LeverConstrainData>();
            _constrainDatas.Add(Handedness.Left, new LeverConstrainData(LeftConstrainReference, LeftConstrainReference.position - touchableObj.position));
            _constrainDatas.Add(Handedness.Right, new LeverConstrainData(RightConstrainReference, RightConstrainReference.position - touchableObj.position));

            touchable = touchableObj.GetComponentInChildren<ITouchable>();
            touchable.OnTouchStay.AddListener(OnInteractionStay);
        }

        protected virtual void OnDestroy()
        {
            touchable.OnTouchStay.RemoveListener(OnInteractionStay);
        }

        public virtual void OnInteractionStay(TouchInteractionData _data)
        {
            if (!isLeverEnable) return;

            var count = 0;
            foreach (FingerTouchInteractionData _fingerData in _data.FingerTouchInteractionDatas.Values)
            {
                if (_fingerData.TouchTarget == touchable && _fingerData.Status == TouchStatus.Touching)
                {
                    count++;
                }
            }

            handedness = _data.Handedness;
            if (count < _FingerCount)
            {
                isHolding = false;
            }
            else
            {
                if (isHolding == false)
                {
                    followObj = _DexmoMotionCtrl.GetTrackTarget(handedness);
                    isHolding = true;
                }
            }

            Rotate();
            ResetReference();     
        }

        /// <summary>
        /// 是否满足旋转的条件
        /// </summary>
        protected override bool CanRotate()
        {
            return followObj != null && isHolding == true;
        }

        /// <summary>
        /// 获取相对于 zeroDirection 转过的角度
        /// </summary>
        /// <returns></returns>
        protected override float GetRotateAngle()
        {
            return AngleTool.GetProjectAngle(followObj.position - RotateReference.position,
                    RotateReference.TransformDirection(zeroDirection), normal, true);
        }

        protected override void OnAngleChange(float angle)
        {
            base.OnAngleChange(angle);
            if (isLimitAngle)
            {
                switch (_leverState)
                {
                    case LeverState.OpenStart:
                        if (angle > LeverOpenAngle)
                        {
                            if (LeverOpenStart != null) LeverOpenStart.Invoke(angle);
                            _leverState = LeverState.Open;
                        }
                        break;
                    case LeverState.Open:
                        if (angle > LeverCloseAngle)
                        {
                            if (LeverOpenEnd != null) LeverOpenEnd.Invoke(angle);
                            _leverState = LeverState.CloseStart;
                        }
                        break;
                    case LeverState.CloseStart:
                        if (angle < LeverCloseAngle)
                        {
                            if (LeverCloseStart != null) LeverCloseStart.Invoke(angle);
                            _leverState = LeverState.Close;
                        }
                        break;
                    case LeverState.Close :
                        if (angle < LeverOpenAngle)
                        {
                            if (LeverCloseEnd != null) LeverCloseEnd.Invoke(angle);
                            _leverState = LeverState.OpenStart;
                        }
                        break;
                }
            }

            if (AngleChange != null) AngleChange.Invoke(angle);
        }

        protected void ResetReference()
        {
            _constrainDatas[handedness].reference.position
                = touchableObj.position + _constrainDatas[handedness].constrainVector;
        }

        protected override void ResetData()
        {
            base.ResetData();
            isHolding = false;
            followObj = null;
            _leverState = LeverState.OpenStart;
        }

        private enum LeverState
        {
            OpenStart,
            Open,
            CloseStart,
            Close,
        }

        private class LeverConstrainData
        {
            public Transform reference;
            public Vector3 constrainVector;

            public LeverConstrainData(Transform reference, Vector3 vector)
            {
                this.reference = reference;
                constrainVector = vector;
            }
        }
    }
}
