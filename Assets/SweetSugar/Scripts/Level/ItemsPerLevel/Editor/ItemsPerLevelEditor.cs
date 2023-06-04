using SweetSugar.Scripts.Items._Interfaces;
using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.Level.ItemsPerLevel.Editor
{
    public class ItemsPerLevelEditor : EditorWindow
    {
        private static GameObject prefab;
        private static int numLevel;
        private static ParticleSystem ps;

        // Add menu item named "My Window" to the Window menu
        public static void ShowWindow(GameObject itemPrefab, int level)
        {
            ps = Resources.Load<ParticleSystem>("Prefabs/Particles/FireworkSplash");
            ItemsPerLevelEditor window = (ItemsPerLevelEditor) EditorWindow.GetWindow(typeof(ItemsPerLevelEditor), true, itemPrefab.name + " editor");

            prefab = Resources.Load<GameObject>("Items/" + itemPrefab.name);
            numLevel = level;
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ItemsPerLevelEditor));
        }

        void OnGUI()
        {
            if (prefab)
            {
                GUILayout.BeginVertical();
                {
                    var sprs = prefab.GetComponent<IColorableComponent>().GetSpritesOrAdd(numLevel);
                    for (var index = 0; index < sprs.Length; index++)
                    {
                        var spr = sprs[index];
                        sprs[index] = (Sprite) EditorGUILayout.ObjectField(spr, typeof(Sprite),true, GUILayout.Width(50), GUILayout.Height(50));
                        if (sprs[index] != spr)
                        {
                            PrefabUtility.SavePrefabAsset(prefab);
                            ps.textureSheetAnimation.SetSprite(index, sprs[index]);
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            else Close();
        }
    }
}