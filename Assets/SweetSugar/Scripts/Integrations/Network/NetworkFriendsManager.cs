
using SweetSugar.Scripts.Core;
using UnityEngine;
#if PLAYFAB || GAMESPARKS || EPSILON
using System.Collections;
#if GAMESPARKS
using SweetSugar.Scripts.Integrations.Network.Gamesparks;
#endif

#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif

#if EPSILON
using EpsilonServer;
#endif


namespace SweetSugar.Scripts.Integrations.Network
{
    /// <summary>
    /// Friends data network manager
    /// </summary>
    public class NetworkFriendsManager
    {
        IFriendsManager friendsManager;

        public NetworkFriendsManager()
        {
#if PLAYFAB
		friendsManager = new PlayFabFriendsManager ();
		 NetworkManager.OnLoginEvent += GetFriends;
#elif GAMESPARKS
            friendsManager = new GameSparksFriendsManager();
             NetworkManager.OnLoginEvent += GetFriends;
#elif EPSILON
            friendsManager = new EpsilonFriendsManager();
#endif
            NetworkManager.OnLogoutEvent += Logout; //1.3.3
        }

        public void Logout()
        {
#if PLAYFAB || GAMESPARKS
            //1.3.3
            NetworkManager.OnLoginEvent -= GetFriends;
#endif
            NetworkManager.OnLogoutEvent -= Logout;
            FacebookManager.THIS.Friends.Clear();
            friendsManager.Logout();
        }


        /// <summary>
        /// Gets the friends list.
        /// </summary>
        public void GetFriends()
        {
            Debug.Log("Get Friends Called..");
            if (!NetworkManager.THIS.IsLoggedIn)
                return;
            Debug.Log("Get Friends Called.. isLoggedIn check complete...");
            if (friendsManager != null)
            {
#if PLAYFAB || GAMESPARKS
                friendsManager.GetFriends(dic =>
                {
                    foreach (var item in dic)
                    {
                        FriendData friend = new FriendData
                        {
                            FacebookID = item.Key,
                            userID = item.Value
                        };
                        Debug.Log(friend.userID);
                        FacebookManager.THIS.AddFriend(friend); //2.1.2
                        //Debug.Log ("    " + item.Key + " == " + item.Value);
                    }

                    FacebookManager.THIS.GetFriendsPicture();
                    PlaceFriendsPositionsOnMap();
                });
                FacebookManager.THIS.AddFriend(FacebookManager.THIS.GetCurrentUserAsFriend()); //2.2.2


#elif EPSILON
                friendsManager.GetFriends(dic =>
                {
                    foreach (var item in dic)
                    {
                        Debug.Log("::  item  :: " + item.Key + " " + item.Value);
                        FriendData friend = FacebookManager.THIS.Friends.Find(i => i.id == item.Key);
                        if (friend == null)
                        {
                            friend = new FriendData();
                            FacebookManager.THIS.AddFriend(friend);
                        }

                        friend.id = item.Key;
                    }
                });
                NetworkManager.friendsManager.PlaceFriendsPositionsOnMap();
#endif
            }
        }

        /// <summary>
        /// Place the friends on map.
        /// </summary>
        public void PlaceFriendsPositionsOnMap()
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            if (friendsManager != null)
            {
                friendsManager.PlaceFriendsPositionsOnMap(list =>
                {
                    foreach (var item in list)
                    {
#if EPSILON
                        FriendData friend =
                            FacebookManager.THIS.Friends.Find(bk => bk.id == item.Key && bk.id != FacebookManager.userID);
                        if (friend != null)
                        {
                            friend.level = item.Value;
                        }
#elif GAMESPARKS || PLAYFAB
                        FriendData friend = FacebookManager.THIS.Friends.Find(bk =>
                            bk.userID == item.Key && bk.userID != NetworkManager.THIS.UserID);
                        if (friend != null)
                        {
                            friend.level = item.Value;
                        }
#endif
                    }

                    NetworkManager.FriendsOnMapLoaded();
                });
            }
        }

        /// <summary>
        /// Gets the leadboard on level.
        /// </summary>
        public void GetLeadboardOnLevel()
        {
            //LevelManager.THIS.StartCoroutine(GetLeadboardCor());
        }

        public IEnumerator GetLeadboardCor(int level)
        {
            NetworkManager.THIS.leadboardList.Clear();
            yield return new WaitUntil(() => NetworkManager.THIS.IsLoggedIn);
            Debug.Log("getting leadboard");

            if (friendsManager != null)
            {
                int LevelNumber = level;
                friendsManager.GetLeadboardOnLevel(LevelNumber, list =>
                {
#if GAMESPARKS || PLAYFAB
                    foreach (var pl in list)
                    {
                        FriendData friend = FacebookManager.THIS.Friends.Find(delegate(FriendData bk)
                            {
                                return bk.userID == pl.userID;
                            }
                        );
                        if (friend != null)
                        {
                            pl.friendData = friend;
                            pl.picture = friend.picture;
                        }

                        LeadboardPlayerData leadboardPlayerData = NetworkManager.THIS.leadboardList.Find(
                            delegate(LeadboardPlayerData bk) { return bk.userID == pl.userID; }
                        );
                        if (leadboardPlayerData != null)
                            leadboardPlayerData = pl;
                        else
                            NetworkManager.THIS.leadboardList.Add(pl);
                        Debug.Log(pl.Name + " " + pl.userID + " " + pl.position + " " + pl.score);
                    }
#elif EPSILON
                    if (list.FindIndex(i=>i.userID == FacebookManager.userID)==-1)
                    {
                        var pl = new LeadboardPlayerData {userID = FacebookManager.userID, position = 1};
                        list.Add(pl);
                    }
                    foreach (var pl in list)
                    {
                        FriendData friend = FacebookManager.THIS.Friends.Find(delegate(FriendData bk)
                            {
                                // Debug.Log("friend name : " + bk.name);
                                return bk.id == pl.userID;
                            }
                        );
                        if (friend != null)
                        {
                            // Debug.Log("friend data : " + friend.level);
                            pl.friendData = friend;
                            pl.picture = friend.pictureSprite;
                        }

                        // LeadboardPlayerData leadboardPlayerData = NetworkManager.leadboardList.Find(
                        //     delegate(LeadboardPlayerData bk) { return bk.userID == pl.userID; }
                        // );
                        // if (leadboardPlayerData != null)
                            // leadboardPlayerData = pl;
                        // else
                            NetworkManager.THIS.leadboardList.Add(pl);
                        // Debug.Log(pl.Name + " " + pl.userID + " " + pl.position + " " + pl.score);
                    }
#endif
                    if (NetworkManager.THIS.leadboardList.Count > 0)
                    {
                        NetworkManager.LevelLeadboardLoaded();
                    }
                });
                //Debug.Log("Leader board count ::: :" + NetworkManager.leadboardList.Count);
            }
        }
    }
}

#endif