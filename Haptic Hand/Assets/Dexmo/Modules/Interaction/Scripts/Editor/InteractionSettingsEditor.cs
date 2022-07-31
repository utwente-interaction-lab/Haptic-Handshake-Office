
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Dexmo.Unity.Interaction
{
    /// <summary>
    /// Editor for interaction settings
    /// </summary>
    [CustomEditor(typeof(InteractionSettings))]
    public class InteractionSettingsEditor : Editor
    {
        /// <summary>
        /// If show the properties for gesture interaction
        /// </summary>
        private bool showGestureSettings = false;

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            InteractionSettings _settings = (InteractionSettings)target;
            showGestureSettings = EditorGUILayout.Foldout(showGestureSettings, "Gesture Interaction Settings", true);
            if (showGestureSettings)
                ShowGestureOptions(_settings);
            if (GUI.changed)
                EditorUtility.SetDirty(_settings);
        }

        /// <summary>
        /// Show the gesture settings option properties on the inspector
        /// </summary>
        /// <param name="settings">Settings for interaction module</param> 
        private void ShowGestureOptions(InteractionSettings settings)
        {
            serializedObject.Update();
            settings.GestureSettings.Enable = EditorGUILayout.Toggle("Enable", settings.GestureSettings.Enable);
            if (settings.GestureSettings.Enable)
            {
                EditorGUILayout.HelpBox("Choose the gestures you want to detect!", MessageType.Warning);
                var datas = InteractionDatabase.Instance.GestureDatas.Gestures;
                var list = settings.GestureSettings.Gestures;
                foreach (var item in datas)
                {
                    if (string.IsNullOrEmpty(item.GestureName))
                        continue;
                    var gestureSelectInfo = list.Find(x => x.GestureName.Equals(item.GestureName));
                    if (gestureSelectInfo == null)
                    {
                        gestureSelectInfo = new GestureData();
                        gestureSelectInfo.GestureName = item.GestureName;
                        list.Add(gestureSelectInfo);
                    }
                    gestureSelectInfo.isEnable
                        = EditorGUILayout.Toggle(string.Format("        {0}", item.GestureName), gestureSelectInfo.isEnable);
                }
                if (list.Count > datas.Count)
                {
                    var tempList = new List<GestureData>(list);

                    for (int i = 0; i < tempList.Count; i++)
                    {
                        var info = datas.Find(x => x.GestureName.Equals(tempList[i].GestureName));
                        if (info == null)
                        {
                            list.Remove(list[i]);
                        }
                    }
                }
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gestureInteractionSettings").FindPropertyRelative("onGestureEnter"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gestureInteractionSettings").FindPropertyRelative("onGestureStay"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gestureInteractionSettings").FindPropertyRelative("onGestureExit"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}