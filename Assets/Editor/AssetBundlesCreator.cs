using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundlesCreator : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")] 
    static void BuildAllAssetBundles() { 
        string path = "Assets/AssetBundles";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, 
            BuildAssetBundleOptions.None, 
            BuildTarget.StandaloneWindows);
    }


}
