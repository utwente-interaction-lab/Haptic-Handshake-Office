using System;
using System.Collections.Generic;
using UnityEngine;
namespace Dexmo.Unity.Model
{
    /// <summary>
    /// Group of hand models. Contains left and right hand models
    /// </summary>
    [Serializable]
    public class HandGroup
    {
        /// <summary>
        /// Name of the hand models group #UnUsed
        /// </summary>
        [Tooltip("Name of the hand group")]
        public string Name;

        /// <summary>
        /// Drag a gameobject with UnityHandModel compoment to set left hand model 
        /// </summary>
        [Tooltip("Left hand model")]
        public UnityHandModel LeftHandModel;

        /// <summary>
        /// Drag a gameobject with UnityHandModel compoment to set right hand model 
        /// </summary>
        [Tooltip("Right hand model")]
        public UnityHandModel RightHandModel;

        /// <summary>
        /// Type of hand model, will set to left and right hand model
        /// </summary>
        [Tooltip("Type of the hand model")]
        public UnityModelType Type;

        /// <summary>
        /// Set whether hand model group is enable, if false the hand models of group will not work 
        /// </summary>
        [Tooltip("Set if the hand model group is enable")]
        public bool Enable = true;

        /// <summary>
        /// Returns hand model by _deviceId
        /// </summary>
        /// <value></value>
        public UnityHandModel this[long _deviceId]
        {
            get
            {
                if (LeftHandModel.DeviceId == _deviceId)
                    return LeftHandModel;
                if (RightHandModel.DeviceId == _deviceId)
                    return RightHandModel;
                return null;
            }
        }

        /// <summary>
        /// Returns hand model by _handedness 
        /// </summary>
        /// <value></value>
        public UnityHandModel this[Handedness _handedness]
        {
            get
            {
                if (LeftHandModel.Handedness == _handedness)
                    return LeftHandModel;
                if (RightHandModel.Handedness == _handedness)
                    return RightHandModel;
                return null;
            }
        }
    }

    /// <summary>
    /// The representation class for hand group. Provides operation method for unity hand models of the HandGroup 
    /// </summary>
    public class HandGroupRepresentation
    {
        /// <summary>
        /// Instance of hand group 
        /// </summary>
        private HandGroup handGroup;

        /// <summary>
        /// Construction method. Init handGroup
        /// </summary>
        /// <param name="_hg">The hand group which will be set to handGroup</param>
        public HandGroupRepresentation(HandGroup _hg)
        {
            if (_hg == null || _hg.LeftHandModel == null || _hg.RightHandModel == null)
            {
                UnityEngine.Debug.unityLogger.LogError("HandGroupRepresentation", "HandGroup should not be empty!");
                return;
            }
            handGroup = _hg;
        }

        /// <summary>
        /// Init all hand models of handGroup
        /// </summary>
        public void InitHandGroup()
        {
            if (handGroup != null)
            {
                handGroup.LeftHandModel.InitHand(handGroup.Type);
                handGroup.RightHandModel.InitHand(handGroup.Type);
            }
        }

        /// <summary>
        /// Start hand model whose device ID equals to _deviceID
        /// </summary>
        /// <param name="_deviceId">ID of Dexmo Device</param>
        public void StartHand(long _deviceId)
        {
            handGroup[_deviceId].StartHand();
        }

        /// <summary>
        /// Update hand model whose device ID equals to _deviceID
        /// </summary>
        /// <param name="_deviceId">Device ID</param>
        public void UpdateHandGroup(long _deviceId)
        {
            handGroup[_deviceId].UpdateHand();
        }

        /// <summary>
        /// Stop hand model whose device ID equals _deviceID
        /// </summary>
        /// <param name="_deviceId"></param>
        public void StopHand(long _deviceId)
        {
            handGroup[_deviceId].StopHand();
        }

        /// <summary>
        /// Destory hand models of hand group
        /// </summary>
        public void DestoryHandGroup()
        {
            if (handGroup != null)
            {
                handGroup.LeftHandModel.DestoryHand();
                handGroup.RightHandModel.DestoryHand();
            }
        }

        /// <summary>
        /// Set _deviceId to device ID of the unity hand model whose Handedness equals to _handedness 
        /// </summary>
        /// <param name="_handedness">Handedness of Dexmo Device</param>
        /// <param name="_deviceId">ID of Dexmo Device</param>
        public void SetHandDeviceId(Handedness _handedness, long _deviceId)
        {
            
            handGroup[_handedness].SetDeviceId(_deviceId);
        }
    }
}