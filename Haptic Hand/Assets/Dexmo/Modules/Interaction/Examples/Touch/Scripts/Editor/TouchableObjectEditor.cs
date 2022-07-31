using UnityEditor;
using Dexmo.Unity.Interaction;
using UnityEngine;

namespace Dexmo.Unity
{
    [CustomEditor(typeof(TouchableObject), true)]
    public class TouchableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();

            TouchableObject _obj = (TouchableObject)serializedObject.targetObject;
            _obj.EnableForceFeedback = EditorGUILayout.Toggle("Enable Force Feedback", _obj.EnableForceFeedback);
            if (_obj.EnableForceFeedback)
            {
                ShowForceFeedbackSettingsPlane(_obj);
            }
            EditorGUILayout.Space();

            _obj.EnableVibrationFeedback = EditorGUILayout.Toggle("Enable Vibration Feedback", _obj.EnableVibrationFeedback);
            if (_obj.EnableVibrationFeedback)
            {
                ShowVibrationFeedbackSettingsPlane(_obj);
            }
            EditorGUILayout.Space();
            ShowTouchUnityEvent();
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(_obj);
        }
        private void ShowForceFeedbackSettingsPlane(TouchableObject _obj)
        {
            EditorGUI.indentLevel += 1;
            GUILayout.BeginVertical();
            _obj.ConstraintOnTouching = EditorGUILayout.Toggle("Constrain Finger On Surface ", _obj.ConstraintOnTouching);
            _obj.PreventShakeOnExtremePosition = EditorGUILayout.Toggle("Prevent Shake On Extreme Position ", _obj.PreventShakeOnExtremePosition);
            _obj.UpdateTouchingPosition = EditorGUILayout.Toggle("Continuously update force feedback position", _obj.UpdateTouchingPosition);
            _obj.FindSurfaceMode = (SurfaceFinderMode)EditorGUILayout.EnumPopup("Surface Finder Mode", _obj.FindSurfaceMode);
            _obj.Stiffness = EditorGUILayout.Slider("Stiffness ", _obj.Stiffness, 0, 1f);
            _obj.LimitRotateBendValueOnSurfaceFinding = EditorGUILayout.Slider("Limit Rotate Bend Value", _obj.LimitRotateBendValueOnSurfaceFinding, 0f, 0.4f);
            _obj.ForceFeedbackDataFilterValue = EditorGUILayout.Slider("Filter Value", _obj.ForceFeedbackDataFilterValue, 0f, 0.1f);
            GUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;
        }

        private void ShowVibrationFeedbackSettingsPlane(TouchableObject _obj)
        {
            EditorGUI.indentLevel += 1;
            GUILayout.BeginVertical();
            _obj.TouchObjType = (TouchObjType)EditorGUILayout.EnumPopup("TouchObject Type:", _obj.TouchObjType);
            if (_obj.TouchObjType == TouchObjType.Impenetrable)
                _obj.SurfaceType = (TouchObjSurface)EditorGUILayout.EnumPopup("SurfaceType:", _obj.SurfaceType);
            _obj.VibrationIntensity = EditorGUILayout.Slider("Vibration Intensity", _obj.VibrationIntensity, 0, 1);
            _obj.EnableForceFeedback = _obj.TouchObjType != TouchObjType.Penetrable;
            GUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;
        }

        private void ShowTouchUnityEvent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onTouchEnter"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onTouchStay"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onTouchExit"), true);
        }
    }
}