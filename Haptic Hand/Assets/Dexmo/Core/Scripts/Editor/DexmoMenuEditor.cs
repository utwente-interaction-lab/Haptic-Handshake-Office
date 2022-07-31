using UnityEditor;
using UnityEngine;
using Dexmo.Unity.Settings;
namespace Dexmo.Unity.EditorExtension
{
    /// <summary>
    /// Menu items on the unity top editor plane. Main menu's default name is "Dexmo"
    /// </summary>
    public class DexmoMenuEditor
    {
        /// <summary>
        /// Menu item root name
        /// </summary>
        private const string MenuRootName = "Dexmo";

        /// <summary>
        /// Settings for Dexmo option menu name
        /// </summary>
        private const string DexmoSettingGameObjectName = "DexmoSettings";

        /// <summary>
        /// Open DexmoSettings script on the inspector window
        /// </summary>
        [MenuItem(MenuRootName + "/Settings/DexmoSetting")]
        public static void OpenDexmoSetting()
        {
            AddAndSelectSettingScript<DexmoSettings>();
        }

        // /// <summary>
        // /// Add ShowFPS script to DexmoManager script's gameobject. If run the non-vr scene, fps will show on the scene
        // /// </summary>
        // [MenuItem(MenuRootName + "/Debug/Show FPS")]
        // public static void ShowFPSFunc()
        // {
        //     AddAndSelectSettingScript<ShowFPS>();
        // }

    
        /// <summary>
        /// Add and select a script in the editor scene 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void AddAndSelectSettingScript<T>() where T : Component
        {
            T _component = GameObject.FindObjectOfType<T>();
            if (_component == null)
            {
                GameObject go = FindDexmoManagerGameObject();
                if (go != null)
                {
                    _component = go.AddComponent<T>();
                }
            }
            // Select the compoment gameobject in scene
            if (_component != null)
            {
                EditorGUIUtility.PingObject(_component.gameObject);
                Selection.activeGameObject = _component.gameObject;
            }
        }

        /// <summary>
        /// Find the gameObject with DexmoManager component. all the menu used scripts is add to this gameobject 
        /// </summary>
        /// <returns></returns>
        private static GameObject FindDexmoManagerGameObject()
        {
            var go = GameObject.FindObjectOfType<DexmoManager>();
            if (go == null)
            {
                Debug.unityLogger.LogError("Dexmo", "There is no DexmoManager compoment in scene! Please first drag the prefab named '[Dexmo]' to the scene !");
                return null;
            }
            return go.gameObject;
        }

    }
}
