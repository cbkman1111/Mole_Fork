#if EPSILON
using System;
using System.Collections.Generic;
using System.Linq;
using EpsilonServer.EpsilonClientAPI;
using SweetSugar.Scripts.Integrations.Network;
using SweetSugar.Scripts.Integrations.Network.EpsilonServer;
using SweetSugar.Scripts.MapScripts;
using UnityEngine;
using UnityEngine.Networking;

namespace EpsilonServer
{
    public class EpsilonDataManager : IDataManagerEpsilon
    {
        public ResultObject[] playerLevels;
        /*public void SetPlayerScore(int level, int score, int stars)
        {
            LevelsUpdateRequest levelsUpdateRequest = new LevelsUpdateRequest().addLevel(new LevelsUpdateRequest.Level(level, score, stars));

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "select", levelsUpdateRequest.toJson(), (response) =>
            {
                Debug.Log("GAME Levels update response: " + response.downloadHandler.text);
            });
        }*/
        public void SetLevels(List<EpsilonLevel> levelsToUpdate)
        {
            LevelsUpdateRequest levelsUpdateRequest = new LevelsUpdateRequest();
            foreach (EpsilonLevel l in levelsToUpdate)
            {
                levelsUpdateRequest.addLevel(new LevelsUpdateRequest.Level(l.level, l.score, l.stars));
            }

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "levels", levelsUpdateRequest.toJson(), (response) =>
            {
                Debug.Log("GAME Levels update response: " + response.downloadHandler.text);
            });
        }
        public void SetLevels()
        {
            LevelsUpdateRequest levelsUpdateRequest = new LevelsUpdateRequest();
            int latestLevel = LevelsMap.GetLastestReachedLevel();
            for (int i = 1; i <= latestLevel; i++)
            {
                levelsUpdateRequest.addLevel(new LevelsUpdateRequest.Level(i, PlayerPrefs.GetInt("Score" + i, 0), PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", i), 0)));
            }

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "levels", levelsUpdateRequest.toJson(), (response) =>
            {
                Debug.Log("GAME Levels update response: " + response.downloadHandler.text);
            });
        }
        public void SetLevel(EpsilonLevel level)
        {
            LevelsUpdateRequest levelsUpdateRequest = new LevelsUpdateRequest().addLevel(level.level, level.stars, level.score);

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "levels", levelsUpdateRequest.toJson(), (response) =>
            {
                Debug.Log("GAME Levels update response: " + response.downloadHandler.text);
            });
        }
        public void SetLevel(EpsilonLevel level, Action callback = null)
        {
            LevelsUpdateRequest levelsUpdateRequest = new LevelsUpdateRequest().addLevel(level.level, level.stars, level.score);

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "levels", levelsUpdateRequest.toJson(), (response) =>
            {
                Debug.Log("GAME Levels update response: " + response.downloadHandler.text);
                callback?.Invoke();
            });
        }

        /*public void SetPlayerLevel(int level, Action callback=null)
        {*/
        /*new EpsilonRequest().SetTable("players").SetAttribute("level", level)
            .AddURL("checkField1=playerId").Send(result => { callback?.Invoke();});*/

        /*LevelsUpdateRequest levelsUpdateRequest = new LevelsUpdateRequest();

        int latestLevel = LevelsMap._instance.GetLastestReachedLevel();
        for (int i = 1; i <= latestLevel; i++) {
            levelsUpdateRequest.addLevel(new LevelsUpdateRequest.Level(i, PlayerPrefs.GetInt("Score" + i, 0), PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", i), 0)));
        }

        EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "levels", levelsUpdateRequest.toJson(), (response) =>
        {
            Debug.Log("GAME Levels update response: " + response.downloadHandler.text);
        });
    }*/

        public void GetLevels(Action<ResultObject> Callback)
        {
            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "select", new SelectApiRequest("levels").toJson(), (response) =>
            {
                if (response.result != UnityWebRequest.Result.ConnectionError && !response.downloadHandler.text.Contains("Error") && !response.downloadHandler.text.Contains("Error"))
                {
                    var resultArray = JsonHelper.getJsonArray<ResultObject>(response.downloadHandler.text);
                    if (resultArray != null && resultArray.Length != 0)
                    {
                        NetworkDataManager.LatestReachedLevel = resultArray.Max(i => i.level);
                        if(LevelsMap._mapProgressManager.GetLastLevel()<= NetworkDataManager.LatestReachedLevel)
                            NetworkDataManager.setLevels(resultArray);
                    }
                    Callback(null);
                    /*else if (resultArray.Length == 0)
                    {
                        SetLevel(new EpsilonLevel(1, 0, 0), () => {});
                    }*/
                }
                else
                {
                    Debug.Log("Error Retrieving Score Data: " + response.downloadHandler.text);
                }
            });
        }

        /*public void GetPlayerScore(Action<ResultObject> Callback)
        {
            Debug.Log("Get player score");
            new EpsilonRequest().SetTable("levels").Get(response =>
            {
                if (!response.isNetworkError)
                {
                    var resultArray = JsonHelper.getJsonArray<ResultObject>( response.downloadHandler.text );
                    playerLevels = resultArray;
                    if(resultArray != null && resultArray.Length != 0)
                        Callback(resultArray.Last());
                }
                else
                {
                    Debug.Log("Error Retrieving Score Data...");
                }
            });
        }*/

        //public void SetStars(int Stars, int Level)
        //{
            //new EpsilonRequest().SetTable("levels").SetAttribute("stars", Stars).SetAttribute("level",Level).SetAttribute("score", PlayerPrefs.GetInt("Score" + Level))./*AddURL
            //("&updateIfExists=true&checkField1 =level").*/Send();
        //}

        /*public void GetStars(Action<Dictionary<string, int>> Callback)
        {
            Debug.Log("Get stars");
            if(playerLevels == null || playerLevels.Length == 0)
            {
                new EpsilonRequest().SetTable("levels").Get(response =>
                {
                    if (!response.isNetworkError && !response.downloadHandler.text.Contains("Error"))
                    {
                        var resultArray = JsonHelper.getJsonArray<ResultObject>(response.downloadHandler.text);
                        playerLevels = resultArray;
                        if(resultArray != null && resultArray.Length != 0)
                            Callback(GetLevelsData());
                    }
                    else
                    {
                        Debug.Log("No user data available");
                    }
                });
            }
            else   Callback(GetLevelsData());
        }*/

       /* private Dictionary<string, int> GetLevelsData()
        {
            return playerLevels.ToDictionary(item => item.level.ToString(), item => item.stars);
        }

        public void SetTotalStars()
        {
        }*/

        public void SetBoosterData(Dictionary<string, int> dic)
        {
            BoostsUpdateRequest boostsUpdateRequest = new BoostsUpdateRequest();
            foreach (var dicRecord in dic)
            {
                //new EpsilonRequest().SetTable("boosts").SetAttribute("name", dicRecord.Key).SetAttribute("count",dicRecord.Value)./*AddURL
                //("&updateIfExists=true&checkField1=boostName").*/Send();

                boostsUpdateRequest.addBoost(new BoostsUpdateRequest.Boost(dicRecord.Key, dicRecord.Value));
            }

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "boosts", boostsUpdateRequest.toJson(), (response) => {});
        }

        public void UpdateBoost(string name, int count)
        {
            BoostsUpdateRequest boostsUpdateRequest = new BoostsUpdateRequest();
            boostsUpdateRequest.addBoost(new BoostsUpdateRequest.Boost(name, count));

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "boosts", boostsUpdateRequest.toJson(), (response) =>
            {
                Debug.Log("Boost\"" + name + "\" update response: " + response.downloadHandler.text);
            });
        }

        public void GetBoosterData(Action<ResultObject[]> Callback)
        {
            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "select", new SelectApiRequest("boosts").toJson(), (response) =>
            {
                if (response.result != UnityWebRequest.Result.ConnectionError && !response.downloadHandler.text.Contains("Error"))
                {
                    //Dictionary<string, int> dicBoost = new Dictionary<string, int>();
                    var serverBoosts = JsonHelper.getJsonArray<ResultObject>(response.downloadHandler.text);
                    Callback(serverBoosts);
                    if (serverBoosts != null && serverBoosts.Length != 0)
                    {
                        /*foreach (var srvBoost in serverBoosts)
                        {
                            if (srvBoost.name.Contains("Boost_"))
                            {
                                dicBoost.Add(srvBoost.name, srvBoost.count);
                            }
                        }

                        Callback(dicBoost);*/
                    }
                    else
                    {
                        Debug.Log("No boost data available");
                    }
                }
            });
        }

        public void Logout()
        {
        }
    }
}
#endif