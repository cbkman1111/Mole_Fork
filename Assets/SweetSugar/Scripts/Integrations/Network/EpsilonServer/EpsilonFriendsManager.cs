#if EPSILON
using System;
using System.Collections.Generic;
using System.Linq;
using EpsilonServer.EpsilonClientAPI;
using SweetSugar.Scripts.Integrations;
using SweetSugar.Scripts.Integrations.Network;
using SweetSugar.Scripts.Integrations.Network.EpsilonServer;
using UnityEngine;
using UnityEngine.Networking;

namespace EpsilonServer
{
    public class EpsilonFriendsManager : IFriendsManager
    {
        public void GetFriends(Action<Dictionary<string, string>> Callback)
        {
            /*new EpsilonRequest().SetTable("players").SetAttribute("facebookId", GetFriendsJSON(FacebookManager.THIS.Friends, true)).Get(response =>
            {
                if (!response.isNetworkError && !response.downloadHandler.text.Contains("error") && !response.downloadHandler.text.Contains("Error"))
                {
                    var resultArray = JsonHelper.getJsonArray<ResultObject>( response.downloadHandler.text );
                    if(resultArray != null && resultArray.Length != 0)
                    {
                        Dictionary<string,string> dic = new Dictionary<string, string> ();
                        foreach (var item in resultArray)
                        {
                            dic.Add(item.facebookId.ToString(), "");
                            var friendData = FacebookManager.THIS.Friends.Find(i => i.id == item.facebookId.ToString());
                            if(friendData != null)
                                friendData.level = item.maxLevel;
                        }
                        Callback (dic);
                    }
                }
            });*/

            List<string> friendFBids = GetFriendIds(FacebookManager.THIS.Friends);
            MaxPlayerprogressRequest maxPlayerprogressRequest = new MaxPlayerprogressRequest(friendFBids);

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "maxplayerprogress", maxPlayerprogressRequest.toJson(), (response) =>
            {
                Debug.Log("<color=yellow>Friends result: " + response.downloadHandler.text + "</color>");
                var resultArray = JsonHelper.getJsonArray<ResultObject>(response.downloadHandler.text);
                if (resultArray != null && resultArray.Length != 0)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (var item in resultArray)
                    {
                        dic.Add(item.facebookId.ToString(), "");
                        var friendData = FacebookManager.THIS.Friends.Find(i => i.id == item.facebookId.ToString());
                        if (friendData != null)
                            friendData.level = item.level;
                    }
                    Callback(dic);
                }
            });
        }


        public void PlaceFriendsPositionsOnMap(Action<Dictionary<string, int>> Callback)
        {
            Dictionary<string,int> dic = new Dictionary<string, int> ();
            foreach (var item in FacebookManager.THIS.Friends)
            {
                if(!dic.ContainsKey(item.id))
                    dic.Add(item.id, item.level);
            }
            Callback (dic);
        }

        public void GetLeadboardOnLevel(int LevelNumber, Action<List<LeadboardPlayerData>> Callback)
        {
            Debug.Log("Get leadboard on level");
            var friendsJson = GetFriendIds(FacebookManager.THIS.Friends);
            PlayerprogressRequest playerprogressRequest = new PlayerprogressRequest(LevelNumber, friendsJson);
            List<LeadboardPlayerData> list = new List<LeadboardPlayerData> ();
            new EpsilonRequest().Special("/game/playerprogress").SetAttribute(playerprogressRequest.toJson()).Get
            (response =>
            {
                Debug.Log(">>>  Response :;: " + response.downloadHandler.text);
                if (response.result != UnityWebRequest.Result.ConnectionError && !response.downloadHandler.text.Contains("error") && !response.downloadHandler.text.Contains("Error"))
                {
                    var resultArray = JsonHelper.getJsonArray<ResultObject>( response.downloadHandler.text );
                    if(resultArray != null && resultArray.Length != 0)
                    {
                        int num = 0;
                        foreach (var item in resultArray.OrderByDescending(i=>i.score))
                        {
                            num++;
                            LeadboardPlayerData pl = new LeadboardPlayerData ();
                            pl.Name = FacebookManager.THIS.Friends.FirstOrDefault(i => i.id == item.facebookId)?.first_name;
                            //pl.userID = item.playerId.ToString();
                            pl.userID = item.facebookId;
                            pl.position = num;
                            pl.score = item.score;
                            if(item.score > 0)
                                list.Add(pl);
                        }
                    }
                } 
                Callback (list);
            });
        }
        
        
        private string GetFriendsJSON(List<FriendData> thisFriends, bool playersTable)
        {
            string line = "[";

            foreach (var thisFriend in thisFriends)
            {
                string id = "";
                id = thisFriend.id;
                if (!thisFriend.Equals(thisFriends.Last()) && id != "") id += ",";
                line += id;
            }

            line += "]";
            return line;
        }

        public List<string> GetFriendIds(List<FriendData> thisFriends)
        {
            List<string> result = new List<string>();

            foreach (var thisFriend in thisFriends)
                result.Add(thisFriend.id);

            return result;
        }

        public void Logout()
        {
            FacebookManager.THIS.CallFBLogout();
        }
    }
}
#endif