using UnityEngine;

namespace SweetSugar.Scripts.Integrations.Network.EpsilonServer
{
    public class EpsilonTestAPI : MonoBehaviour
    {
#if EPSILON
        private void OnGUI()
        {
            if (GUILayout.Button("Send score"))
            {
                //NetworkManager.dataManager.SetPlayerScore(5,200,3);
            }

            if (GUILayout.Button("send level"))
            {
                //NetworkManager.dataManager.SetPlayerLevel(3);
            }

            if (GUILayout.Button("Get level"))
            {
                NetworkManager.dataManager.DownloadPlayerData();
            }
            if (GUILayout.Button("Get score"))
            {
                //NetworkManager.dataManager.GetPlayerScore();
            }

            if (GUILayout.Button("get Friends"))
            {
#if FACEBOOK
                FacebookManager.THIS.GetFriendlist();
#endif
            }
        }
#endif
    }
}