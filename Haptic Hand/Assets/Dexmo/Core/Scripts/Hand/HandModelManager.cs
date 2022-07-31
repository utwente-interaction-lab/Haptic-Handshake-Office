using System;
using System.Collections.Generic;
using UnityEngine;
using Dexmo.Unity.DexmoProvider;
using Dexmo.Unity.Motion;
namespace Dexmo.Unity.Model
{
    /// <summary>
    /// Manages all  hand models
    /// </summary>
    public sealed class HandModelManager : MonoBehaviour
    {
        /// <summary>
        /// Array of all hand groups in scene, you can add a new hand group on Inspector plane 
        /// </summary>
        [SerializeField]
        private HandGroup[] handPool;

        /// <summary>
        /// List of all hand group representations
        /// </summary>
        /// <returns></returns>
        private Dictionary<UnityModelType, List<HandGroupRepresentation>> _HandGroupReps
            = new Dictionary<UnityModelType, List<HandGroupRepresentation>>();

        /// <summary>
        /// List of all connected Dexmo Devices
        /// </summary>
        /// <typeparam name="DexmoDevice">Dexmo Device instance</typeparam>
        /// <returns></returns>
        private List<DexmoDevice> dexmoConnectedDevices = new List<DexmoDevice>();

        /// <summary>
        /// Dexmo Status listener instance
        /// </summary>
        private IDexmoStatusListener dexmoStatusListener;

        /// <summary>
        /// Dexmo motion control instance
        /// </summary>
        private IDexmoMotionControl dexmoMotionControl;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            dexmoMotionControl = DexmoManager.Instance.DexmoMotionControl;
            dexmoStatusListener = DexmoManager.Instance.DexmoStatusListener;
            if (dexmoStatusListener == null)
                return;
            dexmoStatusListener.OnFixedUpdate += OnFixedUpdateFrame;
            dexmoStatusListener.OnUpdate += OnUpdateFrame;
            dexmoStatusListener.OnDexmoDeviceConnected += OnDexmoDeviceConnected;
            dexmoStatusListener.OnDexmoDeviceDisConnected += OnDexmoDeviceDisConnected;
            InitHandGroupRep();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            dexmoStatusListener.OnFixedUpdate -= OnFixedUpdateFrame;
            dexmoStatusListener.OnUpdate -= OnUpdateFrame;
            dexmoStatusListener.OnDexmoDeviceConnected -= OnDexmoDeviceConnected;
            dexmoStatusListener.OnDexmoDeviceDisConnected -= OnDexmoDeviceDisConnected;
            dexmoMotionControl.ClearUnityHandModel();
            foreach (var _hgrs in _HandGroupReps.Values)
            {
                foreach (var _hgr in _hgrs)
                {
                    _hgr.DestoryHandGroup();
                }
            }
        }

        /// <summary>
        /// The function is called in every frame on the UnityEngine OnFixedUpdate function
        /// </summary>
        private void OnFixedUpdateFrame()
        {
            if (dexmoConnectedDevices.Count > 0)
            {
                dexmoMotionControl.UpdateLocationMotion();
                dexmoMotionControl.UpdatePostureMotion(HandModelPostureController.PostureUpdateMode.ConstrainRawDataMode);
                foreach (var _dexmoDevice in dexmoConnectedDevices)
                {
                    foreach (HandGroupRepresentation _hgr in _HandGroupReps[UnityModelType.Physics])
                    {
                        _hgr.UpdateHandGroup(_dexmoDevice.DeviceID);
                    }
                    foreach (HandGroupRepresentation _hgr in _HandGroupReps[UnityModelType.Interaction])
                    {
                        _hgr.UpdateHandGroup(_dexmoDevice.DeviceID);
                    }
                }
            }
        }

        /// <summary>
        /// The function is called in every frame on the UnityEngine OnUpdata function
        /// </summary>
        private void OnUpdateFrame()
        {
            foreach (var _dexmoDevice in dexmoConnectedDevices)
            {
                foreach (HandGroupRepresentation _hgr in _HandGroupReps[UnityModelType.Graphics])
                {
                    _hgr.UpdateHandGroup(_dexmoDevice.DeviceID);
                }
            }
        }


        /// <summary>
        /// Init all the hand group representations
        /// </summary>
        private void InitHandGroupRep()
        {
            foreach (UnityModelType _type in Enum.GetValues(typeof(UnityModelType)))
            {
                _HandGroupReps.Add(_type, new List<HandGroupRepresentation>());
            }
            foreach (HandGroup _hg in handPool)
            {
                if (_hg.Enable)
                {
                    HandGroupRepresentation hdrep = new HandGroupRepresentation(_hg);
                    if (!_HandGroupReps[_hg.Type].Contains(hdrep))
                        _HandGroupReps[_hg.Type].Add(hdrep);
                    hdrep.InitHandGroup();
                    dexmoMotionControl.AddUnityHandModel(_hg.LeftHandModel);
                    dexmoMotionControl.AddUnityHandModel(_hg.RightHandModel);
                }
            }
        }


        /// <summary>
        /// The function is called when Dexmo Device is connected
        /// </summary>
        /// <param name="_dexmoDevice">Connected Dexmo Device</param>
        private void OnDexmoDeviceConnected(DexmoDevice _dexmoDevice)
        {
            if (!dexmoConnectedDevices.Contains(_dexmoDevice))
                dexmoConnectedDevices.Add(_dexmoDevice);
            dexmoMotionControl.EnablePostureMotion(_dexmoDevice.Handedness);
            foreach (var rep in _HandGroupReps.Values)
            {
                foreach (var _hgr in rep)
                {
                    _hgr.SetHandDeviceId(_dexmoDevice.Handedness, _dexmoDevice.DeviceID);
                    _hgr.StartHand(_dexmoDevice.DeviceID);
                }
            }
        }

        /// <summary>
        /// The function is called when Dexmo Device is disconnected
        /// </summary>
        /// <param name="_dexmoDevice">DisConnected Dexmo Device</param>
        private void OnDexmoDeviceDisConnected(DexmoDevice _dexmoDevice)
        {
            if (dexmoConnectedDevices.Contains(_dexmoDevice))
                dexmoConnectedDevices.Remove(_dexmoDevice);
            dexmoMotionControl.DisEnablePostureMotion(_dexmoDevice.Handedness);
            foreach (var rep in _HandGroupReps.Values)
            {
                foreach (var _hgr in rep)
                {
                    _hgr.StopHand(_dexmoDevice.DeviceID);
                }
            }
        }
    }
}
