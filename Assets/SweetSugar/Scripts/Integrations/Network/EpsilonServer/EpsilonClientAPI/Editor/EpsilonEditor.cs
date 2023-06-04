using UnityEditor;
using UnityEngine;

namespace EpsilonServer.EpsilonClientAPI.Editor
{
    public class EpsilonEditor : EditorWindow
    {
        private int step;
        private static EpsilonEditor window;
        private string email = "";
        private string apikey;
        private string game_name;
        private string invoice;
        private string game_package;
        private string genre;

        [MenuItem("Epsilon server/Register game")]
        public static void RegisterGame()
        {
            ShowWindow();
        }
        
        [MenuItem("Epsilon server/Setup")]
        public static void ServerSetup()
        {
            Application.OpenURL("https://docs.google.com/document/d/1WUFLHA4ZadxmluIyWlSLZToSMPvDs4vaE6Ag4newTBg/edit");
        }  
        [MenuItem("Epsilon server/Settings")]
        public static void ServerSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Plugins/EPSILON/Resources/ServerSettings.asset");
        }

        private static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            window = (EpsilonEditor) GetWindow(typeof(EpsilonEditor));
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                if (step == 0) ShowStep0();

            }
            EditorGUILayout.EndVertical();
        }

        private void ShowStep0()
        {
            
            GUILayout.Label("Step1. Put your email here:");
            email = EditorGUILayout.TextField("Email",email);
            game_name = EditorGUILayout.TextField("Game name",game_name);
            invoice = EditorGUILayout.TextField("Invoice number",invoice);
            #if EPSILON
            if (GUILayout.Button("Register game") && email != "")
            {
                EpsilonClientAPI.SendEditorRequiest("/system/register_game", "game_name="+game_name+"&email="+email+"&match3=Yes" +"&send_mail=Yes" + "&invoice=" + invoice, result =>
                {
                    if (result != null)
                    {
                        Debug.Log("result ==>  " + result.downloadHandler.text);
                        var resultArray = JsonUtility.FromJson<APIKeys>(result.downloadHandler.text);
                        var serverSettings = AssetDatabase.LoadAssetAtPath<ServerSettings>("Assets/Plugins/EPSILON/Resources/ServerSettings.asset");
                        serverSettings.APIKey = resultArray.api_key;
                        serverSettings.APISecret = resultArray.api_secret;
                        EditorUtility.SetDirty(serverSettings);
                        AssetDatabase.SaveAssets();
                        Selection.activeObject = serverSettings;
                        Debug.Log("Game " + game_name + " created");
                    }
                });
                Close();
            }

            #endif
        }
    }

    class APIKeys
    {
        public string api_key;
        public string api_secret;
    }
}