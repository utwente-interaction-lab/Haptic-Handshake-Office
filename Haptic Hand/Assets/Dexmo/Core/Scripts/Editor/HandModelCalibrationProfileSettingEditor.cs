using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Dexmo.Unity.Settings;
using Dexmo.Unity.Motion;

namespace Dexmo.Unity.EditorExtension
{
    /// <summary>
    /// Override editor for HandModelCalibrationProfileSetting
    /// </summary>
    [CustomEditor(typeof(HandModelCalibrationProfileSetting))]
    public class HandModelCalibrationProfileSettingEditor : UnityEditor.Editor
    {
        /// <summary>
        /// If hand calibration profile plane is foldout
        /// </summary>
        public static bool calibrateHandFoldout;

        /// <summary>
        /// If finger calibration profile plane is foldout
        /// </summary>
        public static bool calibrateFingerFoldout;

        /// <summary>
        /// Type of finger
        /// </summary>
        private FingerType fingerType;

        /// <summary>
        /// Implement this function to make a custom inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            
            HandModelCalibrationProfileSetting _handProfile =
                (HandModelCalibrationProfileSetting)serializedObject.targetObject;

            SerializedProperty calibrateHandFoldout = serializedObject.FindProperty("handProfile");
            calibrateHandFoldout.isExpanded = EditorGUILayout.Foldout(calibrateHandFoldout.isExpanded, "Hand Calibration");
            if (calibrateHandFoldout.isExpanded)
            {
                ShowCalibrateHandOptions(_handProfile);
            }

            calibrateHandFoldout.isExpanded = EditorGUILayout.Foldout(calibrateHandFoldout.isExpanded, "Finger Calibration");
            if (calibrateHandFoldout.isExpanded)
            { 
                ShowCalibrateFingerOptions(_handProfile);
            }

            EditorGUILayout.LabelField("Data Setting:");
            ShowLoadDataOptions(_handProfile);
            ShowSaveDataOptions(_handProfile);
            GUILayout.Space(10);
        }

        /// <summary>
        /// Delegate functon for hand calibration profile butonn clicked
        /// </summary>
        private delegate void HandProfileMethodForAllFingers();

        /// <summary>
        /// Delegate function for finger calibration profile button clicked
        /// </summary>
        /// <param name="fingerType">Type of finger</param>
        private delegate void HandProfileMethodForOneFinger(FingerType fingerType);

        /// <summary>
        /// Add editor buttons for all the finger calibration profile settings
        /// </summary>
        /// <param name="_handProfile">Hand calibration profile</param>
        /// <param name="label">Name of button</param>
        /// <param name="_handProfileMethodForAllFingers">Delegate function if the button is clicked</param>
        private void AddButtonControlForAllFingers(HandModelCalibrationProfileSetting _handProfile, string label, HandProfileMethodForAllFingers _handProfileMethodForAllFingers)
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button(label))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Transform[] handAndFingers = _handProfile.transform.GetComponentsInChildren<Transform>();
                    Undo.RecordObjects(handAndFingers, "Changed hand and finger transforms.");
                    _handProfileMethodForAllFingers();
                }
            }
        }

        /// <summary>
        /// Add editor button for the finger calibration profile settings. Finger type is fingerType
        /// </summary>
        /// <param name="_handProfile">Hand calibration profile</param>
        /// <param name="label">Name of button</param>
        /// <param name="_handProfileMethodForOneFinger">Delegate function if the button is clicked</param>
        private void AddButtonControlForOneFinger(HandModelCalibrationProfileSetting _handProfile, string label,
            HandProfileMethodForOneFinger _handProfileMethodForOneFinger)
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button(label))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Transform[] handAndFingers = _handProfile.transform.GetComponentsInChildren<Transform>();
                    Undo.RecordObjects(handAndFingers, "Changed finger transform.");
                    _handProfileMethodForOneFinger(fingerType);
                }
            }
        }

        /// <summary>
        /// Show the han calibration profile option plane on the inspector
        /// </summary>
        /// <param name="_handProfile">Hand calibration profile</param>
        private void ShowCalibrateHandOptions(HandModelCalibrationProfileSetting _handProfile)
        {
            EditorGUILayout.LabelField("Calibrate Data");
            AddButtonControlForAllFingers(_handProfile, "Calibrate Hand Initial Config",
                _handProfile.CalibrateHandInitialConfig);

            AddButtonControlForAllFingers(_handProfile, "Calibrate Hand Split Extreme",
                _handProfile.CalibrateHandSplitExtreme);

            AddButtonControlForAllFingers(_handProfile, "Calibrate Hand Bend Extreme",
                _handProfile.CalibrateHandBendExtreme);

            EditorGUILayout.LabelField("Reset Data");
            AddButtonControlForAllFingers(_handProfile, "Reset Hand to Initial Config",
                _handProfile.ResetHandToInitialConfig);

            AddButtonControlForAllFingers(_handProfile, "Reset Hand to Split Extreme",
                _handProfile.ResetHandToSplitExtreme);

            AddButtonControlForAllFingers(_handProfile, "Reset Hand to Bend Extreme",
                _handProfile.ResetHandToBendExtreme);
        }

        /// <summary>
        /// Show the option of finger calibration data 
        /// </summary>
        /// <param name="handProfile">Hand calibration data</param>
        private void ShowCalibrateFingerOptions(HandModelCalibrationProfileSetting _handProfile)
        {
            fingerType = (FingerType)EditorGUILayout.EnumPopup("Finger Type:", fingerType);

            EditorGUILayout.LabelField("Calibrate Data");
            AddButtonControlForOneFinger(_handProfile, "Calibrate Finger Initial Config",
                _handProfile.CalibrateFingerInitialConfig);

            AddButtonControlForOneFinger(_handProfile, "Calibrate Finger Split Extreme",
                _handProfile.CalibrateFingerSplitExtreme);

            AddButtonControlForOneFinger(_handProfile, "Calibrate Finger Bend Extreme",
                _handProfile.CalibrateFingerBendExtreme);

            if (fingerType == FingerType.THUMB)
            {
                AddButtonControlForOneFinger(_handProfile, "Calibrate Finger Rotate Extreme",
                      _handProfile.CalibrateFingerRotateExtreme);
            }

            EditorGUILayout.LabelField("Reset Data");
            AddButtonControlForOneFinger(_handProfile, "Reset Finger To Initial Config",
                _handProfile.ResetFingerToInitialConfig);

            AddButtonControlForOneFinger(_handProfile, "Reset Finger To Split Extreme",
                _handProfile.ResetFingerToSplitExtreme);

            AddButtonControlForOneFinger(_handProfile, "Reset Finger To Bend Extreme",
                _handProfile.ResetFingerToBendExtreme);

            if (fingerType == FingerType.THUMB)
            {
                AddButtonControlForOneFinger(_handProfile, "Reset Finger To Rotate Extreme",
                      _handProfile.ResetFingerToRotateExtreme);
            }

        }

        /// <summary>
        /// Show save hand calibration data button
        /// </summary>
        /// <param name="_handProfile"></param>
        private void ShowSaveDataOptions(HandModelCalibrationProfileSetting _handProfile)
        {
            AddButtonControlForAllFingers(_handProfile, "Save Data", _handProfile.SaveDexmoData);
        }

        /// <summary>
        /// Show load hand calibration data button
        /// </summary>
        /// <param name="_handProfile">Hand calibration data</param>
        private void ShowLoadDataOptions(HandModelCalibrationProfileSetting _handProfile)
        {
            AddButtonControlForAllFingers(_handProfile, "Load Data", _handProfile.LoadDexmoData);
        }
    }
}