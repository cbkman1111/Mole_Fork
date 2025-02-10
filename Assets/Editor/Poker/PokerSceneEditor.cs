using NUnit.Framework;
using Scenes;
using UnityEditor;
using UnityEngine;

namespace Poker
{
    [CustomEditor(typeof(PokerSceneEditor))]
    public class PokerSceneEditor : Editor
    {
        private SceneGostop scene = null;

        //private List<Card> handCards = new List<Card>();

        private void OnEnable()
        {
            scene = (SceneGostop)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            GUILayout.Label("테스트 텍스트", EditorStyles.boldLabel);
        }
    }
}