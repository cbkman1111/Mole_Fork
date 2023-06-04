#if EPSILON
using EpsilonServer.EpsilonClientAPI;
#endif
using UnityEngine;
using UnityEngine.Networking;

namespace SweetSugar.Scripts.Integrations.Network.EpsilonServer
{
    public class EpsilonManager : ILoginManager
    {
        public string userID;
        // Use this for initialization

        #region AUTHORIZATION

        public void LoginWithFB(string accessToken, string titleId)
        {
#if FACEBOOK
        #if EPSILON
        new EpsilonRequest().Special("/player/authorization").SetAttribute("facebookId="+FacebookManager.userID).Get((response) =>
        {
            if (response.result != UnityWebRequest.Result.ConnectionError)
            {
                var downloadHandlerText = response.downloadHandler.text;

                ResultObject result;
                result = JsonUtility.FromJson<ResultObject>(downloadHandlerText);
                Debug.Log("RESULT ::: " + result);
                if (result != null)
                {
                    userID = FacebookManager.userID;
                    Debug.Log ("Got userID: " + userID);
                    NetworkManager.THIS.UserID = result.playerId; 
                    NetworkManager.THIS.IsLoggedIn = true;
                    Debug.Log("FB Epsilon Login Done...." + result.playerId);
                    FacebookManager.THIS.GetFriendlist();
                    /*EpsilonCurrencyManager epsilonCurrencyManager = new EpsilonCurrencyManager();
                    epsilonCurrencyManager.GetBalance((r) =>
                    {
                        Debug.Log("Balance: "+r);
                    });*/
                }
                else
                {
                    Debug.Log("Authorization error");
                }
            }
        });
#endif
#endif
        }


        public void UpdateName(string userName)
        {
        }

        public bool IsYou(string Id)
        {
            if (Id == FacebookManager.userID)
                return true;
            return false;
        }

        #endregion
    }
}