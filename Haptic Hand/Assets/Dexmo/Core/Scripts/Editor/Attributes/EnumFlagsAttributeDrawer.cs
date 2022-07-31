using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Dexmo.Unity.Attributes
{

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
            }
            else
            {
                Debug.unityLogger.LogError("EnumFlagsAttribut", "EnumFlagsAttribut must modify a Enum field!");
            }
        }
    }
}