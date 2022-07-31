using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dexmo.Unity.Motion;
using System;
using Dexmo.Unity.Model;

namespace Dexmo.Unity.Interaction
{
    /// <summary>
    /// Editor settings for constraint object
    /// </summary>
    [CustomEditor(typeof(ConstrainableObject), true)]
    public class ConstrainableObjectEditor : Editor
    {
        /// <summary>
        /// List of constraint posture datas
        /// </summary>
        /// <typeparam name="Handedness">Handedness of hand</typeparam>
        /// <typeparam name="ConstrainHandPostureData">Data for constrain hand posture</typeparam>
        /// <returns></returns>
        private Dictionary<Handedness, ConstrainHandPostureData> constrainDatas = new Dictionary<Handedness, ConstrainHandPostureData>();

        /// <summary>
        /// List of posture hand data which has constrained
        /// </summary>
        /// <typeparam name="Handedness">Handedness of hand</typeparam>
        /// <typeparam name="HandData">Hand data instance</typeparam>
        /// <returns></returns>
        private Dictionary<Handedness, HandData> handDatas = new Dictionary<Handedness, HandData>();

        #region  HelpBox Text

        private SpecialMotionSettingsHelpBoxText PositionSpecialAxisHelpBoxText;

        private SpecialMotionSettingsHelpBoxText RotationSpecialAxisHelpBoxText;

        private SpecialMotionSettingsHelpBoxText RotatePositionReferenceAlongRotationReferenceHelpBoxText;


        private void InitAllHelpBoxTexts()
        {
            if (PositionSpecialAxisHelpBoxText.Equals(default(SpecialMotionSettingsHelpBoxText)))
            {
                PositionSpecialAxisHelpBoxText = new SpecialMotionSettingsHelpBoxText()
                {
                    SpecialAxisTitleName = "Free translate axis ",
                    AxisMotionFreeSettingsHelpText = "Hand models can free translate around the {0} axis of location reference.",
                    AxisMotionFreePositiveSettingsHelpText = "Hand models can free translate in the positive direction of the {0} axis of location reference.",
                    AxisMotionFreeInverseSettingsHelpText = "Hand models can free translate in the inverse direction of the {0} axis of location reference.",
                    AxisMotionPositiveSettingsHelpText = "Hand models can only translate positive distance around the {0} axis of location reference.",
                    AxisMotionInverseSettingsHelpText = "Hand models can only translate negative distance around the {0} axis of location reference.",
                    AxisMotionInsideBoundsSettingsHelpText = "Hand models can only translate inside the distance bounds of the {0} axis of location reference, the vector represents the local coordinates of the position reference!"
                };
            }

            if (RotationSpecialAxisHelpBoxText.Equals(default(SpecialMotionSettingsHelpBoxText)))
            {
                RotationSpecialAxisHelpBoxText = new SpecialMotionSettingsHelpBoxText()
                {
                    SpecialAxisTitleName = "Free self rotate axis ",
                    AxisMotionFreeSettingsHelpText = "Hand models can free self rotate around the {0} axis of location reference.",
                    AxisMotionFreePositiveSettingsHelpText = "Hand models can free self rotate in the clockwise direction of the {0} axis of location reference.",
                    AxisMotionFreeInverseSettingsHelpText = "Hand models can free self rotate in the counterclockwise direction of the {0} axis of location reference.",
                    AxisMotionPositiveSettingsHelpText = "Hand models can only self rotate clockwise angel around the {0} axis of location reference.",
                    AxisMotionInverseSettingsHelpText = "Hand models can only self rotate counterclockwise angle around the {0} axis of location reference.",
                    AxisMotionInsideBoundsSettingsHelpText = "Hand models can only self rotate the angle which is inside the angle bounds of the {0} axis of location reference!"
                };
            }
            if (RotatePositionReferenceAlongRotationReferenceHelpBoxText.Equals(default(SpecialMotionSettingsHelpBoxText)))
            {
                RotatePositionReferenceAlongRotationReferenceHelpBoxText = new SpecialMotionSettingsHelpBoxText()
                {
                    SpecialAxisTitleName = "Free globle rotate axis ",
                    AxisMotionFreeSettingsHelpText = "Hand model can free globle rotate around the {0} axis of rotation reference.",
                    AxisMotionFreePositiveSettingsHelpText = "Hand model can free globle rotate in the clockwise direction of the {0} axis of rotation reference.",
                    AxisMotionFreeInverseSettingsHelpText = "Hand model can free globle rotate in the counterclockwise direction of the {0} axis of rotation reference.",
                    AxisMotionPositiveSettingsHelpText = "Hand model can only globle rotate clockwise angel around the {0} axis of rotation reference.",
                    AxisMotionInverseSettingsHelpText = "Hand model can only globle rotate counterclockwise angle around the {0} axis of rotation reference.",
                    AxisMotionInsideBoundsSettingsHelpText = "Hand model can only globle rotate the angle which is inside the angle bounds of the {0} axis of rotation reference!"
                };
            }
        }

        #endregion

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            InitAllHelpBoxTexts();
            ConstrainableObject _constrainObject = (ConstrainableObject)target;
            InspectorSetting(_constrainObject);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onConstraintEnter"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onConstraintStay"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onConstraintExit"), true);
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        #region Inspector

        private void InspectorSetting(ConstrainableObject _constrainObj)
        {
            _constrainObj.PreviewPostureData = EditorGUILayout.Toggle("Preview Posture", _constrainObj.PreviewPostureData);
            if (_constrainObj.PreviewPostureData)
            {
                EditorGUI.indentLevel += 1;
                _constrainObj.handModel_L
                    = (UnityHandModel)EditorGUILayout.ObjectField("Left Hand: ", _constrainObj.handModel_L, typeof(UnityHandModel), true);
                _constrainObj.handModel_R
                    = (UnityHandModel)EditorGUILayout.ObjectField("Right Hand: ", _constrainObj.handModel_R, typeof(UnityHandModel), true);
                EditorGUI.indentLevel -= 1;
                InitDatas();
                if (GUI.changed)
                {
                    UpdateHandPosture(_constrainObj, Handedness.Left);
                    UpdateHandPosture(_constrainObj, Handedness.Right);
                }
            }
            SerializedProperty _constrainLeftHand = serializedObject.FindProperty("LeftHandConstraintData");
            _constrainLeftHand.isExpanded = EditorGUILayout.Foldout(_constrainLeftHand.isExpanded, "Constrain LeftHand");
            if (_constrainLeftHand.isExpanded)
                ShowHandConstrainData(Handedness.Left, _constrainObj.LeftHandConstraintData, _constrainLeftHand);

            SerializedProperty _constrainRightHand = serializedObject.FindProperty("RightHandConstraintData");
            _constrainRightHand.isExpanded = EditorGUILayout.Foldout(_constrainRightHand.isExpanded, "Constrain RightHand");
            if (_constrainRightHand.isExpanded)
                ShowHandConstrainData(Handedness.Right, _constrainObj.RightHandConstraintData, _constrainRightHand);
        }

        private void ShowHandConstrainData(Handedness handedness, ConstrainHandData data, SerializedProperty _serializedProperty)
        {
            EditorGUI.indentLevel += 1;
            SerializedProperty _locationConstraint = _serializedProperty.FindPropertyRelative("LocationData");
            _locationConstraint.isExpanded = EditorGUILayout.Foldout(_locationConstraint.isExpanded, "LocationConstraint");
            if (_locationConstraint.isExpanded)
                ShowLocationConstrainData(data.LocationData, _locationConstraint);

            SerializedProperty _postureConstraint = _serializedProperty.FindPropertyRelative("PostureData");
            _postureConstraint.isExpanded = EditorGUILayout.Foldout(_postureConstraint.isExpanded, "PostureConstraint");
            if (_postureConstraint.isExpanded)
                ShowPostureConstrainData(handedness, data.PostureData);
            EditorGUI.indentLevel -= 1;
        }

        private void ShowLocationConstrainData(ConstrainHandLocationData locationData, SerializedProperty _serializedProperty)
        {
            EditorGUI.indentLevel += 1;
            locationData.InvalidDistance = EditorGUILayout.Slider("Invalid Distance: ", locationData.InvalidDistance, 0, 1);

            SerializedProperty _initialRotationSettings = _serializedProperty.FindPropertyRelative("HandModelRotationInitialSettings").FindPropertyRelative("CanInverseRotation");
            _initialRotationSettings.isExpanded = EditorGUILayout.Foldout(_initialRotationSettings.isExpanded, "HandModel Initial Rotation Settings");
            if (_initialRotationSettings.isExpanded)
            {
                ShowInitialSettingsData(locationData);
            }

            locationData.ConstrainPosition.EnableConstrain
                = EditorGUILayout.ToggleLeft("Postion Constraint: ", locationData.ConstrainPosition.EnableConstrain);
            if (locationData.ConstrainPosition.EnableConstrain)
            {
                ShowConstrainDataSetting(locationData.ConstrainPosition, PositionSpecialAxisHelpBoxText);
            }

            locationData.ConstrainRotation.EnableConstrain
                = EditorGUILayout.ToggleLeft("Rotation Constraint: ", locationData.ConstrainRotation.EnableConstrain);
            if (locationData.ConstrainRotation.EnableConstrain)
            {
                ShowConstrainDataSetting(locationData.ConstrainRotation, RotationSpecialAxisHelpBoxText);
                if (locationData.ConstrainPosition.EnableConstrain && locationData.ConstrainPosition.Reference != null && locationData.ConstrainRotation.Reference != null &&
                            locationData.ConstrainPosition.Reference != locationData.ConstrainRotation.Reference)
                    ShowRotatePositionReferenceSettings(locationData.ConstrainRotation);
            }
            EditorGUI.indentLevel -= 1;
        }

        private void ShowInitialSettingsData(ConstrainHandLocationData locationData)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.BeginVertical();
            locationData.HandModelRotationInitialSettings.CanInverseRotation = EditorGUILayout.Toggle("Can inverse rotation", locationData.HandModelRotationInitialSettings.CanInverseRotation);
            locationData.HandModelRotationInitialSettings.StayRotateReferenceRotation = EditorGUILayout.Toggle("Stay with rotate reference's rotation", locationData.HandModelRotationInitialSettings.StayRotateReferenceRotation);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;
        }

        private void ShowRotatePositionReferenceSettings(RotationConstraintDataSettings _rotationSettings)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Position reference is not equal to Rotation reference. You can set hand model rotate around the axis of rotation reference!", MessageType.Warning);
            if (_rotationSettings.RotatePositionReferenceAxisSettings == null)
                _rotationSettings.RotatePositionReferenceAxisSettings = new SpecialMotionSettings();
            ShowSpecialMotionSettingsPlane(_rotationSettings.RotatePositionReferenceAxisSettings, RotatePositionReferenceAlongRotationReferenceHelpBoxText, false);
            if (_rotationSettings.RotatePositionReferenceAxisSettings.Axis != SpecialAxis.None)
                _rotationSettings.CaculateRotateAngleWithPositionReference = EditorGUILayout.Toggle("Caculate rotate angle by position of position reference", _rotationSettings.CaculateRotateAngleWithPositionReference);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;
        }

        private void ShowConstrainDataSetting(PositionConstraintDataSettings settings, SpecialMotionSettingsHelpBoxText _specialAxisHelpBoxText)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.BeginVertical();
            settings.Reference =
               (Transform)EditorGUILayout.ObjectField("Reference: ", settings.Reference, typeof(Transform), true);
            if (settings.Reference != null)
            {
                if (settings.IgnoreAxisSettings == null)
                    settings.IgnoreAxisSettings = new SpecialMotionSettings();
                ShowSpecialMotionSettingsPlane(settings.IgnoreAxisSettings, _specialAxisHelpBoxText);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;
        }

        private void ShowSpecialMotionSettingsPlane(SpecialMotionSettings _specialAxisSettings, SpecialMotionSettingsHelpBoxText _helpBoxText, bool _changeIndentLevel = true)
        {
            if (_changeIndentLevel)
                EditorGUI.indentLevel += 1;
            string _titleName = _helpBoxText.Equals(default(SpecialMotionSettingsHelpBoxText)) ? "Special Axis " : _helpBoxText.SpecialAxisTitleName;
            _specialAxisSettings.Axis = (SpecialAxis)EditorGUILayout.EnumPopup(_titleName, _specialAxisSettings.Axis);
            SpecialAxis _axis = _specialAxisSettings.Axis;
            if (_specialAxisSettings.Axis != SpecialAxis.None)
            {
                if (!_helpBoxText.Equals(default(SpecialMotionSettingsHelpBoxText)))
                {
                    switch (_specialAxisSettings.AxisMotionSettings)
                    {
                        case SpecialAxisMotionSettings.Free:
                            EditorGUILayout.HelpBox(string.Format(_helpBoxText.AxisMotionFreeSettingsHelpText, _axis), MessageType.None);
                            break;
                        case SpecialAxisMotionSettings.FreePositive:
                            EditorGUILayout.HelpBox(string.Format(_helpBoxText.AxisMotionFreePositiveSettingsHelpText, _axis), MessageType.None);
                            break;
                        case SpecialAxisMotionSettings.FreeInverse:
                            EditorGUILayout.HelpBox(string.Format(_helpBoxText.AxisMotionFreeInverseSettingsHelpText, _axis), MessageType.None);
                            break;
                        case SpecialAxisMotionSettings.Positive:
                            EditorGUILayout.HelpBox(string.Format(_helpBoxText.AxisMotionPositiveSettingsHelpText, _axis), MessageType.None);
                            break;
                        case SpecialAxisMotionSettings.Inverse:
                            EditorGUILayout.HelpBox(string.Format(_helpBoxText.AxisMotionInverseSettingsHelpText, _axis), MessageType.None);
                            break;
                        case SpecialAxisMotionSettings.InsideBounds:
                            EditorGUILayout.HelpBox(string.Format(_helpBoxText.AxisMotionInsideBoundsSettingsHelpText, _axis), MessageType.None);
                            break;
                    }
                }

                _specialAxisSettings.AxisMotionSettings = (SpecialAxisMotionSettings)EditorGUILayout.EnumPopup(_titleName + "bounds", _specialAxisSettings.AxisMotionSettings);
                if (_specialAxisSettings.AxisMotionSettings == SpecialAxisMotionSettings.InsideBounds)
                {
                    EditorGUI.indentLevel += 1;
                    Vector2 _boundsVec2 = _specialAxisSettings.AxisMotionBounds;
                    _boundsVec2.x = EditorGUILayout.FloatField("Bounds-min:", _boundsVec2.x);
                    _boundsVec2.y = EditorGUILayout.FloatField("Bounds-max:", _boundsVec2.y);
                    _specialAxisSettings.AxisMotionBounds = _boundsVec2;
                    EditorGUI.indentLevel -= 1;
                }
            }
            if (_changeIndentLevel)
                EditorGUI.indentLevel -= 1;
        }

        private static bool[] fingerFoldoutStates = { false, false, false, false, false };
        private void ShowPostureConstrainData(Handedness handedness, ConstrainHandPostureData postureData)
        {
            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel += 2;
            foreach (FingerType finger in Enum.GetValues(typeof(FingerType)))
            {
                fingerFoldoutStates[(int)finger] = EditorGUILayout.Foldout(fingerFoldoutStates[(int)finger], finger.ToString());
                if (fingerFoldoutStates[(int)finger])
                {
                    ShowFingerPostureConstrainData(finger, postureData[finger]);
                }
            }

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        private void ShowFingerPostureConstrainData(FingerType finger, ConstrainFingerPostureData data)
        {
            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel += 1;

            data.SetBendConstraintEnable(EditorGUILayout.ToggleLeft("Bend", data.EnableConstrainBend));
            if (data.EnableConstrainBend)
            {
                ShowJointBendConstrainData(data);
            }
            data.SetSplitConstraintEnable(EditorGUILayout.ToggleLeft("Split", data.EnableConstrainSplit));
            if (data.EnableConstrainSplit)
            {
                float _minSplitValue = data.MinFingerData.SplitValue;
                float _maxSplitValue = data.MaxFingerData.SplitValue;
                ShowMinMaxValue(ref _minSplitValue, ref _maxSplitValue);
                data.MinFingerData.SetSplitValue(_minSplitValue);
                data.MaxFingerData.SetSplitValue(_maxSplitValue);
            }

            if (finger == FingerType.THUMB)
            {
                data.SetRotateConstraintEnable(EditorGUILayout.ToggleLeft("Rotate", data.EnableConstrainRotate));
                if (data.EnableConstrainRotate)
                {
                    float _minRotateValue = data.MinFingerData.RotateValue;
                    float _maxRotateValue = data.MaxFingerData.RotateValue;
                    ShowMinMaxValue(ref _minRotateValue, ref _maxRotateValue);
                    data.MinFingerData.SetRotateValue(_minRotateValue);
                    data.MaxFingerData.SetRotateValue(_maxRotateValue);
                }
            }

            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        private void ShowJointBendConstrainData(ConstrainFingerPostureData data)
        {
            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel += 1;
            foreach (JointType joint in Enum.GetValues(typeof(JointType)))
            {
                data.ConstrainJointsBend[(int)joint] = EditorGUILayout.ToggleLeft(joint.ToString(), data.ConstrainJointsBend[(int)joint]);
                if (data.ConstrainJointsBend[(int)joint])
                {
                    ShowMinMaxValue(ref data.MinFingerData[joint].BendValue, ref data.MaxFingerData[joint].BendValue);
                }
            }
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        private void ShowMinMaxValue(ref float min, ref float max)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 1);
            EditorGUILayout.LabelField("min:", min.ToString());
            EditorGUILayout.LabelField("max:", max.ToString());
            EditorGUI.indentLevel -= 1;
        }



        #endregion

        #region Preview

        /// <summary>
        /// Init constraint data and posture hand datas
        /// </summary>
        private void InitDatas()
        {
            if (constrainDatas.Count == 0)
            {
                constrainDatas.Add(Handedness.Left, new ConstrainHandPostureData());
                constrainDatas.Add(Handedness.Right, new ConstrainHandPostureData());
            }
            if (handDatas.Count == 0)
            {
                handDatas.Add(Handedness.Left, new HandData());
                handDatas.Add(Handedness.Right, new HandData());
            }
        }

        /// <summary>
        /// Show the preview options plane
        /// </summary>
        /// <param name="constrainObj">Constraint object</param>
        private void ShowPreviewOptions(ConstrainableObject constrainObj)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset LeftHand Gesture"))
            {
                PostureReset(constrainObj, Handedness.Left);
            }

            if (GUILayout.Button("Reset LeftHand Data"))
            {
                ResetData(constrainObj, Handedness.Left);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset RightHand Gesture"))
            {
                PostureReset(constrainObj, Handedness.Right);
            }
            if (GUILayout.Button("Reset RightHand Data"))
            {
                ResetData(constrainObj, Handedness.Right);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Update the hand model posture by constraint data from the constrain object
        /// </summary>
        /// <param name="_constrainObj">Constraint object</param>
        /// <param name="_handedness">Handedness of hand model</param>
        private void UpdateHandPosture(ConstrainableObject _constrainObj, Handedness _handedness)
        {
            var handModel = _handedness == Handedness.Left ? _constrainObj.handModel_L : _constrainObj.handModel_R;
            if (handModel != null)
            {
                foreach (var _fingerModel in handModel.FingerModels)
                {
                    if (null == _fingerModel.JointModels || _fingerModel.JointModels.Length == 0)
                        _fingerModel.InitFinger(handModel.Handedness);
                    PosturePreview(_constrainObj, _handedness, _fingerModel, _fingerModel.Type);
                }
            }
            SaveConstrainHandPostureData(_constrainObj, _handedness);
        }

        /// <summary>
        /// Save the constraint posture datas
        /// </summary>
        /// <param name="_constrainObject">Constraint object</param>
        /// <param name="_handedness">Handedness of hand</param>
        private void SaveConstrainHandPostureData(ConstrainableObject _constrainObject, Handedness _handedness)
        {
            constrainDatas[_handedness].Copy(_constrainObject.GetHandPostureData(_handedness));
        }

        private void PosturePreview(ConstrainableObject _constrainObj, Handedness _handedness, Model.UnityFingerModel fingerModel,
            FingerType _fingerType)
        {
            var _FingerProfile = HandModelProfileProvider.GetHandModelProfile(_handedness)[_fingerType];
            if (_FingerProfile != null)
            {
                var _constrainFinger = _constrainObj.GetHandPostureData(_handedness)[_fingerType];
                var _lastConstrainFinger = constrainDatas[_handedness][_fingerType];
                var _saveFingerData = handDatas[_handedness][_fingerType];

                foreach (JointType _jointType in System.Enum.GetValues(typeof(JointType)))
                {
                    JointRotateData data = new JointRotateData(_fingerType, _jointType, _FingerProfile);

                    //Split Constrain
                    if (_constrainFinger.EnableConstrainSplit)
                    {
                        if (_constrainFinger.MinFingerData.SplitValue != _lastConstrainFinger.MinFingerData.SplitValue)
                        {
                            _saveFingerData.SetSplitValue(_constrainFinger.MinFingerData.SplitValue);
                        }
                        else if (_constrainFinger.MaxFingerData.SplitValue != _lastConstrainFinger.MaxFingerData.SplitValue)
                        {
                            _saveFingerData.SetSplitValue(_constrainFinger.MaxFingerData.SplitValue);
                        }
                    }

                    //Rotate Constrain
                    if (_constrainFinger.EnableConstrainRotate)
                    {
                        if (_constrainFinger.MinFingerData.RotateValue != _lastConstrainFinger.MinFingerData.RotateValue)
                        {
                            _saveFingerData.SetRotateValue(_constrainFinger.MinFingerData.RotateValue);
                        }
                        else if (_constrainFinger.MaxFingerData.RotateValue != _lastConstrainFinger.MaxFingerData.RotateValue)
                        {
                            _saveFingerData.SetRotateValue(_constrainFinger.MaxFingerData.RotateValue);
                        }
                    }

                    var _savedJointData = _saveFingerData[_jointType];
                    //Bend Constrain
                    if (_constrainFinger.EnableConstrainBend)
                    {
                        if (_constrainFinger.ConstrainJointsBend[(int)_jointType])
                        {
                            if (_constrainFinger.MinFingerData[_jointType].BendValue != _lastConstrainFinger.MinFingerData[_jointType].BendValue)
                            {
                                _savedJointData.BendValue = _constrainFinger.MinFingerData[_jointType].BendValue;
                            }
                            else if (_constrainFinger.MaxFingerData[_jointType].BendValue != _lastConstrainFinger.MaxFingerData[_jointType].BendValue)
                            {
                                _savedJointData.BendValue = _constrainFinger.MaxFingerData[_jointType].BendValue;
                            }
                        }
                    }

                    data.UpdateRotateValue(_savedJointData.BendValue, _saveFingerData.SplitValue, _saveFingerData.RotateValue);
                    fingerModel.transform.UpdatePosture(_fingerType, _jointType, data);
                }
            }
        }

        /// <summary>
        /// Reset the hand model posture to constraint data from constraint object
        /// </summary>
        /// <param name="_constrainObj">Constraint object</param>
        /// <param name="_handedness">Handedness of hand model</param>
        private void PostureReset(ConstrainableObject _constrainObj, Handedness _handedness)
        {
            var handModel = _handedness == Handedness.Left ? _constrainObj.handModel_L : _constrainObj.handModel_R;
            if (handModel != null)
            {
                var _HandModelProfile = HandModelProfileProvider.GetHandModelProfile(_handedness);
                for (int i = 0; i < handModel.FingerModels.Length; i++)
                {
                    var finger = handModel.FingerModels[i];
                    FingerType _fingerType = (FingerType)i;
                    _constrainObj.GetHandPostureData(_handedness)[_fingerType].DisEnableConstraint();
                    foreach (JointType _jointType in System.Enum.GetValues(typeof(JointType)))
                    {
                        var _JointProfile = _HandModelProfile[_fingerType][_jointType];
                        if (_JointProfile != null)
                        {
                            finger[_jointType].transform.localRotation = _JointProfile.InitialConfigRotation;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reset the constraint data from constraint object to initial status
        /// </summary>
        /// <param name="_constrainObj">Constraint object</param>
        /// <param name="_handedness">Handedness of hand model</param>
        private void ResetData(ConstrainableObject _constrainObj, Handedness _handedness)
        {
            PostureReset(_constrainObj, _handedness);
            _constrainObj.GetHandPostureData(_handedness).Copy(new ConstrainHandPostureData());
        }

        #endregion
    }
    public struct SpecialMotionSettingsHelpBoxText
    {
        public string SpecialAxisTitleName;
        public string AxisMotionFreeSettingsHelpText;
        public string AxisMotionFreePositiveSettingsHelpText;
        public string AxisMotionFreeInverseSettingsHelpText;
        public string AxisMotionPositiveSettingsHelpText;
        public string AxisMotionInverseSettingsHelpText;
        public string AxisMotionInsideBoundsSettingsHelpText;
    }
}