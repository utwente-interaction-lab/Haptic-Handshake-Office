using System;
using UnityEditor;
using UnityEngine;
namespace Dexmo.Unity.Attributes
{
    [CustomPropertyDrawer(typeof(ConditionalVisibleAttribute))]
    public class ConditionHideInspectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //get the attribute data
            ConditionalVisibleAttribute condHAtt = (ConditionalVisibleAttribute)attribute;
            //check if the propery we want to draw should be enabled
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property, condHAtt.ConditionValue);
            //Enable/disable the property
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            //Check if we should draw the property
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            //Ensure that the next property that is being drawn uses the correct settings
            GUI.enabled = wasEnabled;
        }
        private bool GetConditionalHideAttributeResult(ConditionalVisibleAttribute condHAtt, SerializedProperty property, string value)
        {
            bool enabled = true;
            //Look for the sourcefield within the object that the property belongs to
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField);
            if (sourcePropertyValue != null)
            {
                switch (sourcePropertyValue.type)
                {
                    case "bool":
                        enabled = sourcePropertyValue.boolValue.ToString().Equals(value);
                        break;
                    case "Enum":
                        int enumIndex = sourcePropertyValue.enumValueIndex;
                        enabled = sourcePropertyValue.enumNames[enumIndex].Equals(value);
                        break;
                }
            }
            else
            {
                Debug.unityLogger.LogError("ConditionHideInspectorAttributeDrawer:", "Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }
            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalVisibleAttribute condHAtt = (ConditionalVisibleAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property, condHAtt.ConditionValue);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                //The property is not being drawn
                //We want to undo the spacing added before and after the property
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}