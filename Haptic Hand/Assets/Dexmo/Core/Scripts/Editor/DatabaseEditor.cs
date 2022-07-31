using UnityEngine;
using UnityEditor;
namespace Dexmo.Unity.EditorExtension
{
    /// <summary>
    /// Editor for Dexmo Database
    /// </summary>
    public class DatabaseEditor : Editor
    {
        /// <summary>
        /// If inspector plane of Dexmo Databas is in debug mode. If true the properties of inspector can be changed, false otherwise
        /// </summary>
        private bool isDebugMode = false;

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            GUI.enabled = isDebugMode;
            base.OnInspectorGUI();
        }

        /// <summary>
        /// Implement this function to make a custom header.
        /// </summary>
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            if (GUI.Button(new Rect(45, 22, 50, 18), isDebugMode ? "Apply" : "Editor"))
            {
                DexmoDatabase.Instance.SaveDatabase();
                isDebugMode = !isDebugMode;
            }
        }
    }
}