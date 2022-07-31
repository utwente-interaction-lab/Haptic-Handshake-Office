using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dexmo.Unity.Interaction
{
    public class ButtonBase : MonoBehaviour
    {
        [SerializeField]
        protected bool isAutoRelease = true;
        [SerializeField]
        protected ButtonMover buttonMover;
        [SerializeField]
        private TouchableObject _touchableObj;

        public ButtonStatus _status;
        public ButtonStatus Status { get { return _status; } }

        protected virtual void Start()
        {
            if (_touchableObj != null)
            {
                _touchableObj.OnTouchEnter.AddListener(_PressStartChecker);
                _touchableObj.OnTouchStay.AddListener(_PressStayChecker);
                _touchableObj.OnTouchExit.AddListener(ReleaseChecker);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_touchableObj != null)
            {
                _touchableObj.OnTouchEnter.RemoveListener(_PressStartChecker);
                _touchableObj.OnTouchStay.RemoveListener(_PressStayChecker);
                _touchableObj.OnTouchExit.RemoveListener(ReleaseChecker);
            }
        }

        protected virtual void _PressStartChecker(TouchInteractionData data)
        {
            //Debug.Log("enter");
            _status = ButtonStatus.Pressing;
        }

        protected virtual void _PressStayChecker(TouchInteractionData data)
        {
            if (isAutoRelease == false && _status == ButtonStatus.PressDown)
                return;

            //Debug.Log( _status == ButtonStatus.Releasing ? "release" : "stay");

            buttonMover.FollowTriggeringBody();
            UpdateButtonStatus();
        }

        protected virtual void ReleaseChecker(TouchInteractionData data)
        {
            if (_status == ButtonStatus.PressDown)
            {
                if (isAutoRelease)
                {
                    _status = ButtonStatus.Releasing;
                    _SwitchToSoftState();
                    //如果手指穿透button
                    if (!buttonMover.CheckOverlappingWithTriggeringBody())
                        buttonMover.MoveToStartPos();
                }
            }
            else
            {
                buttonMover.MoveToStartPos();
                _status = ButtonStatus.NotPress;
            }
        }

        private void UpdateButtonStatus()
        {
            if (buttonMover.CurPositionNormalized >= 0.98f)
            {
                if (_status == ButtonStatus.Releasing) return;

                if (_status != ButtonStatus.PressDown)
                {
                    _SwitchToHardState();
                    ButtonClick();
                    _status = ButtonStatus.PressDown;
                }
            }
            else if (buttonMover.CurPositionNormalized >= 0.02f)
            {
                _status = ButtonStatus.Pressing;
            }
            else
            {
                _status = ButtonStatus.NotPress;
            }
        }

        private void _SwitchToHardState()
        {
            Debug.Log("hard");
            _touchableObj.transform.position = buttonMover.BtnEndPos.position;
            _touchableObj.ConstraintOnTouching = true;
            buttonMover.MoveToEndPos();
        }

        private void _SwitchToSoftState()
        {
            Debug.Log("soft");
            _touchableObj.ConstraintOnTouching = false;
            _touchableObj.transform.position = buttonMover.BtnStartPos.position;
            buttonMover.MoveToEndPos();
        }

        protected virtual void ButtonClick()
        {
            Debug.Log("ButtonClick");
        }
    }

    public enum ButtonStatus
    {
        NotPress,
        Pressing,
        PressDown,
        Touching,
        Releasing,
    }
}
