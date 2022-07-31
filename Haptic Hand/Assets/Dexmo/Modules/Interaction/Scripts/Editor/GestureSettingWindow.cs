using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Dexmo.Unity.Model;
using Dexmo.Unity.Motion;
using Dexmo.Unity.Interaction;

namespace Dexmo.Unity.EditorExtension
{
    /// <summary>
    /// Editor window for gesture settings
    /// </summary>
    public class GestureSettingWindow : EditorWindow
    {
        /// <summary>
        /// Width of the window
        /// </summary>
        private float windowWidth;

        /// <summary>
        /// Offset value of the window
        /// </summary>
        private float windowOffset = 15;

        /// <summary>
        /// Scorll position of the window
        /// </summary>
        private Vector2 scorllPosition;

        /// <summary>
        /// Gesture hand data which will be delay delete
        /// </summary>
        private GestureHandData delayDeleteData;

        /// <summary>
        /// Gesture hand data which will be delay reset
        /// </summary>
        private GestureHandData delayResetData;

        /// <summary>
        /// List of window scroll positions
        /// </summary>
        /// <typeparam name="GestureHandData">Gesture data of hand</typeparam>
        /// <typeparam name="Vector2">Scroll position</typeparam>
        /// <returns></returns>
        private Dictionary<GestureHandData, Vector2> scrollPositions = new Dictionary<GestureHandData, Vector2>();

        /// <summary>
        /// If gesture data of hand is visible on the window
        /// </summary>
        /// <typeparam name="GestureHandData">Gesture data of hand</typeparam>
        /// <typeparam name="bool">If ture hand gesture data is visible on the window</typeparam>
        /// <returns></returns>
        private Dictionary<GestureHandData, bool> handGestureVisible = new Dictionary<GestureHandData, bool>();

        /// <summary>
        /// If finger gesture data is visible on the window
        /// </summary>
        /// <typeparam name="GestureHandData">Gesture data of hand</typeparam>
        /// <typeparam name="bool[]">If ture finger gesture data whose type equals index of array is visible on the window </typeparam>
        /// <returns></returns>
        private Dictionary<GestureHandData, bool[]> fingerGestureVisible = new Dictionary<GestureHandData, bool[]>();

        /// <summary>
        /// Preview gesture data instances
        /// </summary>
        /// <typeparam name="GestureHandData">Gesture hand data</typeparam>
        /// <typeparam name="GesturePreviewData">Preview gesture data of gesture</typeparam>
        /// <returns></returns>
        private Dictionary<GestureHandData, GesturePreviewData> previewDatas = new Dictionary<GestureHandData, GesturePreviewData>();

        /// <summary>
        /// GUI update
        /// </summary>
        private void OnGUI()
        {
            windowWidth = this.position.width;
            DrawGestureData();
            DrawGestureDataEditorTool();
        }

        #region Draw GestureData

        /// <summary>
        /// Draw gesture data on the window
        /// </summary>
        private void DrawGestureData()
        {
            using (var scope = new EditorGUILayout.ScrollViewScope(scorllPosition))
            {
                scorllPosition = scope.scrollPosition;
                foreach (var data in InteractionDatabase.Instance.GestureDatas.Gestures)
                {
                    using (var vscope = new EditorGUILayout.VerticalScope())
                    {
                        GUI.Box(vscope.rect, new GUIContent());
                        DrawDataHeader(data);
                        if (IsDrawFingerData(data))
                            DrawFingerData(data);
                    }
                }
                if (delayDeleteData != null)
                {
                    InteractionDatabase.Instance.GestureDatas.Gestures.Remove(delayDeleteData);
                    delayDeleteData = null;
                }
                if (delayResetData != null)
                {
                    if (previewDatas.ContainsKey(delayResetData))
                        previewDatas[delayResetData].gestureData.Copy(new GestureHandData());
                    delayResetData.Copy(new GestureHandData());
                    delayResetData = null;
                }
            }
        }

        /// <summary>
        /// Draw a header plane on the hand gesture data area 
        /// </summary>
        /// <param name="data"></param>
        private void DrawDataHeader(GestureHandData data)
        {
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.gray;
                GUI.Box(scope.rect, "");
                GUI.backgroundColor = Color.white;

                EditorGUILayout.LabelField("GestureName", GUILayout.Width(80));

                data.GestureName = EditorGUILayout.TextField(data.GestureName, GUILayout.Width(windowWidth - 80 - 30 - windowOffset));

                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    delayDeleteData = data;
                }
                GUI.backgroundColor = Color.white;
            }
        }

        /// <summary>
        /// Returns if 
        /// </summary>
        /// <param name="data">Hand gesture data</param>
        /// <returns></returns>
        private bool IsDrawFingerData(GestureHandData data)
        {
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                GUI.Box(scope.rect, "");
                if (!handGestureVisible.ContainsKey(data))
                {
                    handGestureVisible.Add(data, false);
                }
                handGestureVisible[data] = EditorGUILayout.Toggle("FingerData", handGestureVisible[data]);
                return handGestureVisible[data];
            }
        }

        /// <summary>
        /// Draw finger gesture data
        /// </summary>
        /// <param name="data">Hand gesture data</param>
        private void DrawFingerData(GestureHandData data)
        {
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                GUI.Box(scope.rect, "");
                if (!scrollPositions.ContainsKey(data))
                    scrollPositions.Add(data, Vector2.zero);

                if (!fingerGestureVisible.ContainsKey(data))
                    fingerGestureVisible.Add(data, new bool[5]);

                using (var scope2 = new EditorGUILayout.ScrollViewScope(scrollPositions[data], GUILayout.Height(EditorGUIUtility.singleLineHeight * 15)))
                {
                    scrollPositions[data] = scope2.scrollPosition;
                    SetGestureData(data);
                    PreviewHandGesture(data);
                }
            }
        }

        /// <summary>
        /// Set hand gesture data equals to data
        /// </summary>
        /// <param name="data">The hand gesture data which will be set</param>
        private void SetGestureData(GestureHandData data)
        {
            var showFinger = fingerGestureVisible[data];
            foreach (FingerType item in Enum.GetValues(typeof(FingerType)))
            {
                int index = (int)item;

                EditorGUILayout.BeginHorizontal();
                showFinger[index] = EditorGUILayout.Toggle(string.Format("              {0}", item.ToString()), showFinger[index]);
                if (showFinger[index])
                {
                    data[item].EnableSplitAxis = EditorGUILayout.Toggle("               Split", data[item].EnableSplitAxis);
                }
                EditorGUILayout.EndHorizontal();

                if (showFinger[index])
                {
                    var bend = data[item].Bend[JointType.MCP];

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("                            MinBendValue");
                    bend.ValueMin = EditorGUILayout.Slider(bend.ValueMin, 0, 1);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("                            MaxBendValue");
                    bend.ValueMax = EditorGUILayout.Slider(bend.ValueMax, 0, 1);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    if (data[item].EnableSplitAxis)
                    {
                        var split = data[item].Split;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("                            MinSplitValue");
                        split.ValueMin = EditorGUILayout.Slider(split.ValueMin, 0, 1);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("                            MaxSplitValue");
                        split.ValueMax = EditorGUILayout.Slider(split.ValueMax, 0, 1);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        #endregion

        #region Preview HandGesture

        /// <summary>
        /// Preview hand gesture data on the unity hand model in scene
        /// </summary>
        /// <param name="data"></param>
        private void PreviewHandGesture(GestureHandData data)
        {
            if (!previewDatas.ContainsKey(data))
            {
                GesturePreviewData previewData = new GesturePreviewData();
                previewDatas.Add(data, previewData);
            }

            if (previewDatas[data].isView = EditorGUILayout.Toggle("Is Preview HandPosture", previewDatas[data].isView))
            {
                previewDatas[data].handModel = (UnityHandModel)EditorGUILayout.ObjectField("HandModel", previewDatas[data].handModel, typeof(UnityHandModel), true);
                UpdateHandPosture(data);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Reset Gesture Data"))
                {
                    ResetHandPosture(data, true);
                }
                if (GUILayout.Button("Reset Hand Gesture without Reset Data"))
                {
                    ResetHandPosture(data);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Update joint model Posture
        /// </summary>
        private void UpdateHandPosture(GestureHandData data)
        {
            if (previewDatas[data].handModel != null)
            {
                var handModel = previewDatas[data].handModel;
                for (int i = 0; i < handModel.FingerModels.Length; i++)
                {
                    var finger = handModel.FingerModels[i];
                    foreach (JointType _jointType in Enum.GetValues(typeof(JointType)))
                    {
                        UpdateJointPosture(data, handModel.Handedness, finger[_jointType].transform, finger.Type, _jointType);
                    }
                }
            }
        }

        /// <summary>
        /// Update joint model Posture
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_handedness"></param>
        /// <param name="_target"></param>
        /// <param name="_fingerType"></param>
        /// <param name="_jointType"></param>
        private void UpdateJointPosture(GestureHandData _data, Handedness _handedness, Transform _target, FingerType _fingerType, JointType _jointType)
        {
            var _FingerProfile = HandModelProfileProvider.GetHandModelProfile(_handedness)[_fingerType];
            var _JointProfile = _FingerProfile[_jointType];

            if (_JointProfile != null)
            {
                JointRotateData data = new JointRotateData(_fingerType, _jointType, _FingerProfile);

                if (_jointType == JointType.MCP)
                {
                    var _FingerData = _data[_fingerType];
                    if (_FingerData != null)
                    {
                        var _lastFingerData = previewDatas[_data].gestureData[_fingerType];
                        var _SplitData = _FingerData.Split;
                        if (_FingerData.EnableSplitAxis)
                        {
                            if (_SplitData.ValueMin != _lastFingerData.Split.ValueMin)
                            {
                                _SplitData.LastChangeValue = _SplitData.ValueMin;
                            }
                            else if (_SplitData.ValueMax != _lastFingerData.Split.ValueMax)
                            {
                                _SplitData.LastChangeValue = _SplitData.ValueMax;
                            }

                            if (_SplitData.LastChangeValue != _lastFingerData.Split.LastChangeValue)
                            {
                                previewDatas[_data].gestureData.Copy(_data);
                            }
                        }

                        var _BendData = _FingerData.Bend[_jointType];
                        var _LastBendData = _lastFingerData.Bend[_jointType];

                        if (_BendData.ValueMin != _LastBendData.ValueMin)
                        {
                            _BendData.LastChangeValue = _BendData.ValueMin;
                        }
                        else if (_BendData.ValueMax != _LastBendData.ValueMax)
                        {
                            _BendData.LastChangeValue = _BendData.ValueMax;
                        }

                        if (_BendData.LastChangeValue != _LastBendData.LastChangeValue)
                        {
                            previewDatas[_data].gestureData.Copy(_data);
                        }
                    }
                }

                var gestureData = previewDatas[_data].gestureData[_fingerType];
                var bendValue = gestureData.Bend[_jointType].LastChangeValue;
                var splitValue = gestureData.Split.LastChangeValue;

                data.UpdateRotateValue(bendValue, splitValue, gestureData.Rotate.LastChangeValue);
                _target.UpdatePosture(_fingerType, _jointType, data);
            }
        }

        /// <summary>
        /// Reset hand model posture to the initial rotation from joint calibration profile
        /// </summary>
        /// <param name="_handGestureData">Gesture hand data</param>
        /// <param name="isResetData">If reset gesture data</param>
        private void ResetHandPosture(GestureHandData _handGestureData, bool isResetData = false)
        {
            var handModel = previewDatas[_handGestureData].handModel;
            if (handModel != null)
            {
                var _handedness = handModel.Handedness;
                var _HandModelProfile = HandModelProfileProvider.GetHandModelProfile(_handedness);

                foreach (var _fingerModel in handModel.FingerModels)
                {
                    FingerType _fingerType = _fingerModel.Type;
                    foreach (JointType _jointType in Enum.GetValues(typeof(JointType)))
                    {
                        var _JointProfile = _HandModelProfile[_fingerType][_jointType];
                        if (_JointProfile != null)
                        {
                            _fingerModel[_jointType].transform.localRotation = _JointProfile.InitialConfigRotation;
                        }
                    }
                }
            }

            if (isResetData)
            {
                delayResetData = _handGestureData;
            }
            else
            {
                previewDatas[_handGestureData].handModel = null;
                previewDatas[_handGestureData].isView = false;
            }
        }

        #endregion
        /// <summary>
        /// Draw the editor buttons which used to debug gesture data on the window
        /// </summary>
        private void DrawGestureDataEditorTool()
        {
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("New Gesture Data", GUILayout.Width(120)))
                {
                    InteractionDatabase.Instance.GestureDatas.Gestures.Add(new GestureHandData());
                }
                EditorGUILayout.LabelField("", GUILayout.Width(windowWidth - 240 - windowOffset));
                if (GUILayout.Button("Save Gesture Data", GUILayout.Width(120)))
                {
                    DexmoDatabase.Instance.SaveDatabase();
                }
            }
        }

        /// <summary>
        /// Preview gesture data by the unity hand model in scene
        /// </summary>
        private class GesturePreviewData
        {
            /// <summary>
            /// If preview gesture data
            /// </summary>
            public bool isView;

            /// <summary>
            /// Target unity hand model
            /// </summary>
            public UnityHandModel handModel;

            /// <summary>
            /// Gesture hand data which will be previewed
            /// </summary>
            public GestureHandData gestureData;

            /// <summary>
            /// Construction method. Init properties
            /// </summary>
            public GesturePreviewData()
            {
                isView = false;
                handModel = null;
                gestureData = new GestureHandData();
            }
        }
    }
    public class GestureEditorExtension
    {
        [MenuItem("Dexmo/Settings/GestureSetting")]
        public static void ShowEditorWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<GestureSettingWindow>("Gesture Data", true);
            window.position = new Rect(200, 300, 400, 400);
            window.maxSize = new Vector2(1000, 1000);
        }
    }
}