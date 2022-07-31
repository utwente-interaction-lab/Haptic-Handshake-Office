using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DexmoPackageSetting : AssetPostprocessor
{
    static Dictionary<string, int> AddLayerInfo = new Dictionary<string, int>() { { "Hand", 0 }, { "HandInteractable", 0 }};
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string s in importedAssets)
        {
            if (s.Contains("DexmoPackageSetting.cs"))
            {
                Debug.Log("Welcome Dexmo World!!");
                bool initLayersSuccess = InitDexmoLayers();
                if (initLayersSuccess)
                    InitDexmoLayersCollision();
                return;
            }
        }
    }

    private static bool InitDexmoLayers()
    {
        List<string> keyList = new List<string>(AddLayerInfo.Keys);
        int tempLayerIndex = 0;
        string tempLayerName = "";
        for (int i = 0; i < keyList.Count; ++i)
        {
            tempLayerName = keyList[i];
            if (AddLayer(tempLayerName, out tempLayerIndex))
            {
                AddLayerInfo[tempLayerName] = tempLayerIndex;
            }
            else
            {
                Debug.unityLogger.LogError("Dexmo", string.Format("Init {0} layer failed! Please check the version of unity!", tempLayerName));
                return false;
            }
        }
        return true;
    }

    private static void InitDexmoLayersCollision()
    {
        IgnoreAllLayersCollision(AddLayerInfo["Hand"], false, new List<int>() { AddLayerInfo["HandInteractable"] });
        IgnoreAllLayersCollision(AddLayerInfo["HandInteractable"], true, new List<int>() { AddLayerInfo["Hand"] });
    }

    static void AddTag(string tag)
    {
        if (!isHasTag(tag))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "tags")
                {
                    it.arraySize++;
                    SerializedProperty dataPoint = it.GetArrayElementAtIndex(it.arraySize - 1);
                    dataPoint.stringValue = tag;
                    tagManager.ApplyModifiedProperties();
                }
            }
        }
    }

    static bool AddLayer(string layer, out int layerIndex)
    {
        layerIndex = 0;
        if (!isHasLayer(layer))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "layers")
                {
                    for (int i = 8; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                        if (string.IsNullOrEmpty(dataPoint.stringValue))
                        {
                            dataPoint.stringValue = layer;
                            tagManager.ApplyModifiedProperties();
                            layerIndex = LayerMask.NameToLayer(layer);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        else
        {
            layerIndex = LayerMask.NameToLayer(layer);
            return true;
        }
    }
    static void IgnoreAllLayersCollision(int layerIndex, bool ignoreBaseLayers, List<int> ignoreLayerIndex = null)
    {
        for (int i = 0; i < 8; ++i)
            Physics.IgnoreLayerCollision(layerIndex, i, ignoreBaseLayers);

        foreach (int index in AddLayerInfo.Values)
        {
            if (ignoreLayerIndex == null || !ignoreLayerIndex.Contains(index))
                Physics.IgnoreLayerCollision(layerIndex, index, true);
            else
                Physics.IgnoreLayerCollision(layerIndex, index, false);
        }
    }

    static bool isHasTag(string tag)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.tags[i].Equals(tag))
                return true;
        }
        return false;
    }

    static bool isHasLayer(string layer)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.layers[i].Equals(layer))
                return true;
        }
        return false;
    }
}