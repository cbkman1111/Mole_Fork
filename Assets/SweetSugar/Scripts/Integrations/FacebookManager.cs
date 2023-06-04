using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Integrations.Network;
using SweetSugar.Scripts.Integrations.Network.EpsilonServer;
using SweetSugar.Scripts.MapScripts.StaticMap.Editor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
#if FACEBOOK
using Facebook.Unity;
#endif

namespace SweetSugar.Scripts.Integrations
{
    /// <summary>
    /// Facebook manager
    /// </summary>
    public class FacebookManager : MonoBehaviour
    {
        private bool LoginEnable;

        public static string userID;
        public List<FriendData> Friends = new List<FriendData>();

        public string LastResponse { get; set; } = string.Empty;

        public string Status { get; set; } = "Ready";

        bool loginForSharing;
        public static FacebookManager THIS;

        bool loginOnce;
        //2.1.3

        void Awake()
        {
            if (THIS == null) THIS = this;
            else if (THIS != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this);
#if FACEBOOK
            FacebookManager.THIS.CallFBInit();
#endif
        }

        void OnEnable()
        {
#if PLAYFAB
            NetworkManager.OnLoginEvent += GetUserName;

#endif
        }


        void OnDisable()
        {
#if PLAYFAB
            NetworkManager.OnLoginEvent -= GetUserName;

#endif
        }

        public void AddFriend(FriendData friend)
        {
#if PLAYFAB || GAMESPARKS
            //2.1.2
            FriendData friendIndex = Friends.Find(delegate(FriendData bk) { return bk.userID == friend.userID; });
            if (friendIndex == null)
                Friends.Add(friend);
#elif EPSILON
            if (Friends == null) return;
            FriendData friendIndex = Friends.Find(bk => bk.id == friend.id);
            if (friendIndex == null)
                Friends.Add(friend);
#endif
        }

        public void SetPicture(string userID, Sprite sprite)
        {
            //2.1.2
#if PLAYFAB || GAMESPARKS
            FriendData friendIndex = Friends.Find(delegate(FriendData bk) { return bk.userID == userID; });
            if (friendIndex != null)
                friendIndex.picture = sprite;
#elif EPSILON
            if (Friends == null) return;
            FriendData friendIndex = Friends.FirstOrDefault(bk => bk.id == userID);
            if (friendIndex != null)
                friendIndex.pictureSprite = sprite;
#endif
        }

        public FriendData GetCurrentUserAsFriend()
        {
#if PLAYFAB || GAMESPARKS
            FriendData friend = new FriendData
            {
                FacebookID = NetworkManager.THIS.facebookUserID,
                userID = NetworkManager.THIS.UserID,
                picture = NetworkManager.profilePic
            };

            /*FriendData friend = new FriendData
            {
                id = NetworkManager.facebookUserID,
                userID = NetworkManager.UserID,
                //picture = NetworkManager.profilePic
                pictureSprite = NetworkManager.profilePic
            };*/
            //		print ("playefab id: " + friend.PlayFabID);
            return friend;
#elif EPSILON
            FriendData friend = new FriendData
            {
                id = NetworkManager.THIS.facebookUserID,
                pictureSprite = NetworkManager.profilePic
            };
            StartCoroutine(CoroutineUtils.WaitUntil(() => NetworkManager.profilePic != null, () => friend.pictureSprite
                = NetworkManager.profilePic));
            return friend;
#else
            Debug.Log("Please setup PlayFab, gamesparks OR EPSILON ");
            return null;
#endif
        }

        #region FaceBook_stuff

#if FACEBOOK
        public void CallFBInit()
        {
            Debug.Log("init facebook");
            if (!FB.IsInitialized)
            {
                FB.Init(OnInitComplete, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }
        }

        private void OnInitComplete()
        {
            Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
            if (FB.IsLoggedIn)
            {
                //1.3
                LoggedSuccefull(); //2.1.3
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            Debug.Log("Is game showing? " + isGameShown);
        }

        void OnGUI()
        {
            if (LoginEnable)
            {
                CallFBLogin();
                LoginEnable = false;
            }
        }

        public void CallFBLogin()
        {
            if (!loginOnce)
            {
                //2.1.3
                loginOnce = true;
                Debug.Log("login");
                FB.LogInWithReadPermissions(new List<string> {"public_profile", "email", "user_friends"}, HandleResult);
            }
        }

        public void CallFBLogout()
        {
            FB.LogOut();

#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.profilePic = null;
            NetworkManager.THIS.IsLoggedIn = false;
#endif
            SceneManager.LoadScene(Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName());
        }

        public void Share()
        {
            if (!FB.IsLoggedIn)
            {
                loginForSharing = true;
                LoginEnable = true;
                Debug.Log("not logged, logging");
            }
            else
            {
                FB.FeedShare(
                    link: new Uri("https://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" +
                                  (FB.IsLoggedIn ? AccessToken.CurrentAccessToken.UserId : "guest")),
                    linkName: "Sweet Sugar",
                    linkCaption: "I've got " + LevelManager.Score + " scores! Try to beat me!"
                );
            }
        }

        protected void HandleResult(IResult result)
        {
            loginOnce = false; //2.1.3
            if (result == null)
            {
                LastResponse = "Null Response\n";
                Debug.Log(LastResponse);
                return;
            }

            // Some platforms return the empty string instead of null.
            if (!string.IsNullOrEmpty(result.Error))
            {
                Status = "Error - Check log for details";
                LastResponse = "Error Response:\n" + result.Error;
                Debug.Log(result.Error);
            }
            else if (result.Cancelled)
            {
                Status = "Cancelled - Check log for details";
                LastResponse = "Cancelled Response:\n" + result.RawResult;
                Debug.Log(result.RawResult);
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                Status = "Success - Check log for details";
                LastResponse = "Success Response:\n" + result.RawResult;
                LoggedSuccefull(); //1.3
            }
            else
            {
                LastResponse = "Empty Response\n";
                Debug.Log(LastResponse);
            }
        }

        public void LoggedSuccefull()
        {
#if PLAYFAB || GAMESPARKS || EPSILON
		NetworkManager.THIS.IsLoggedIn = true;
#endif
            PlayerPrefs.SetInt("Facebook_Logged", 1);
            PlayerPrefs.Save();


            //Debug.Log(result.RawResult);
            userID = AccessToken.CurrentAccessToken.UserId;
            GetPicture(AccessToken.CurrentAccessToken.UserId);

#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.THIS.facebookUserID = AccessToken.CurrentAccessToken.UserId;
            NetworkManager.THIS.LoginWithFB(AccessToken.CurrentAccessToken.TokenString);
#endif
        }

        void GetUserName()
        {
            FB.API("/me?fields=first_name", HttpMethod.GET, GettingNameCallback);
        }

        private void GettingNameCallback(IGraphResult result)
        {
            if (string.IsNullOrEmpty(result.Error))
            {
                IDictionary dict = result.ResultDictionary as IDictionary;
                string fbname = dict["first_name"].ToString();

#if PLAYFAB || GAMESPARKS || EPSILON
                NetworkManager.THIS.UpdateName(fbname);
#endif
            }
        }

        IEnumerator loadPicture(string url) //2.1.4
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            www.downloadHandler = texDl;
            yield return www.SendWebRequest();

            var texture = texDl.texture;

            var sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
            
#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.profilePic = sprite;
            SetPicture(NetworkManager.THIS.UserID, NetworkManager.profilePic);
            NetworkManager.PlayerPictureLoaded();
#endif
        }


        void GetPicture(string id)
        {
            FB.API("/" + id + "/picture?g&width=128&height=128&redirect=false", HttpMethod.GET,
                ProfilePhotoCallback); //2.1.4
        }

        private void ProfilePhotoCallback(IGraphResult result)
        {
#if EPSILON
            if (string.IsNullOrEmpty(result.Error)) //2.1.4
            {
                StartCoroutine(loadPicture(JsonUtility.FromJson<Picture>(result.RawResult).data.url));
            }
#endif
#if PLAYFAB || GAMESPARKS
            if (string.IsNullOrEmpty(result.Error)) //2.1.4
            {
                var dic = result.ResultDictionary["data"] as Dictionary<string, object>;
                string url = dic.First(i => i.Key == "url").Value as string;
                StartCoroutine(loadPicture(url));
            }
#endif
        }

#if EPSILON
        public void GetFriendlist()
        {
            FB.API("me/friends?fields=name,first_name,id,picture.width(128).height(128)", HttpMethod.GET,
                RequestFriendlistCallback); //2.1.6
        }

        private void RequestFriendlistCallback(IGraphResult result)
        {
            Debug.Log("result.RawResult ::: " + result.RawResult);
            var friendsData = JsonUtility.FromJson<FriendsArray>(result.RawResult);
//		Friends.Clear();
            foreach (var friend in friendsData.data)
                Friends.Add(friend);
            AddFriend(FacebookManager.THIS.GetCurrentUserAsFriend()); //2.2.2
            foreach (var friendData in Friends)
            {
                if (friendData.picture != null)
                    GetPictureByURL(friendData.id, friendData);
            }

#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.friendsManager.GetFriends();
#endif
        }

#endif

        public void GetFriendsPicture()
        {
            FB.API("me/friends?fields=picture.width(128).height(128)", HttpMethod.GET, RequestFriendsCallback); //2.1.6
        }

        private void RequestFriendsCallback(IGraphResult result)
        {
            if (!string.IsNullOrEmpty(result.RawResult))
            {
                var resultDictionary = result.ResultDictionary;
                if (resultDictionary.ContainsKey("data"))
                {
                    var dataArray = (List<object>) resultDictionary["data"]; //2.1.4
                    var dic = dataArray.Select(x => x as Dictionary<string, object>).ToArray();

                    foreach (var item in dic)
                    {
                        string id = (string) item["id"];
                        var url = item.Where(x => x.Key == "picture")
                            .SelectMany(x => x.Value as Dictionary<string, object>).Where(x => x.Key == "data")
                            .SelectMany(x => x.Value as Dictionary<string, object>).Where(i => i.Key == "url").First()
                            .Value;
#if GAMESPARKS || PLAYFAB
                        FriendData friend = Friends.Where(x => x.FacebookID == id).FirstOrDefault();
                        if (friend != null)
                            GetPictureByURL("" + url, friend);
#elif EPSILON
                        FriendData friend = Friends.Where(x => x.id.ToString() == id).FirstOrDefault();
                        if (friend != null)
                        {
                            Debug.Log("friend ::: " + friend.ToString());
                            GetPictureByURL("" + url, friend);
                        }
#endif
                    }
                }

                if (!string.IsNullOrEmpty(result.Error))
                {
                    Debug.Log(result.Error);
                }
            }
        }

        public void GetPictureByURL(string url, FriendData friend)
        {
            StartCoroutine(GetPictureCor(url, friend));
        }

        IEnumerator GetPictureCor(string url, FriendData friend)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            www.downloadHandler = texDl;
            yield return www.SendWebRequest();

            /*if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {*/
            // Get downloaded asset bundle
            //var texture = DownloadHandlerTexture.GetContent(www);
            var sprite = Sprite.Create(texDl.texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
#if GAMESPARKS || PLAYFAB
            friend.picture = sprite;
#elif EPSILON
            friend.pictureSprite = sprite;
#endif
            //   }
            //		print ("get picture for " + url);
        }

        public void APICallBack(IGraphResult result)
        {
            Debug.Log(result);
        }

#endif

        #endregion
    }

#if PLAYFAB
    [Serializable]
    public class FriendData
    {
        public string userID;
        public string FacebookID;
        public Sprite picture;
        public int level;
        public GameObject avatar;
    }
#elif EPSILON
    [Serializable]
    public class FriendData
    {
        public string name;
        public string userID;
        public string first_name;

        public string playerId;

        //Facebook id
        public string id; //info: id and FacebookID is the same.
        public Sprite pictureSprite;
        public Sprite picture;
        public int level;
        public GameObject avatar;
    }
#else
    [Serializable]
    public class FriendData
    {
        public string userID;
        public string FacebookID;
        public Sprite picture;
        public int level;
        public GameObject avatar;
    }

#endif

#if EPSILON
    [Serializable]
    public class Picture
    {
        public PictureData data;
    }

    [Serializable]
    public class PictureData
    {
        public string url;
    }

    [Serializable]
    public class FriendsArray
    {
        public List<FriendData> data;
    }
#endif
}