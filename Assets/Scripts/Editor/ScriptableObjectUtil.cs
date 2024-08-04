using UnityEditor;
using UnityEngine;

namespace Match3
{
    public class ScriptableObjectUtil : MonoBehaviour
    {
#if UNITY_EDITOR
        public static T CreateAsset<T>(string path, string name) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }
#endif
    }
}

