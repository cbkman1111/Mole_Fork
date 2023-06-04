
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.GUI.Boost;
using SweetSugar.Scripts.MapScripts;
using UnityEngine;
#if PLAYFAB || GAMESPARKS || EPSILON
using SweetSugar.Scripts.Integrations.Network.EpsilonServer;
using System.Collections.Generic;
#if GAMESPARKS
using SweetSugar.Scripts.Integrations.Network.Gamesparks;
#endif
#if PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
#endif

#if EPSILON
using EpsilonServer;
#endif

namespace SweetSugar.Scripts.Integrations.Network
{
    /// <summary>
    /// Player data network manager
    /// </summary>
     public class NetworkDataManager
    {
#if PLAYFAB || GAMESPARKS
        public IDataManager dataManager;
#elif EPSILON
        public IDataManagerEpsilon dataManager;
        public static List<EpsilonLevel> levels;
#endif
        public static Dictionary<string, int> remoteBoosts;
        public static int LatestReachedLevel;
        public static int LevelScoreCurrentRecord;

        public NetworkDataManager()
        {
#if PLAYFAB
            dataManager = new PlayFabDataManager();
#elif GAMESPARKS
            dataManager = new GamesparksDataManager();
#elif EPSILON
            dataManager = new EpsilonDataManager();
#endif


#if PLAYFAB || GAMESPARKS
            NetworkManager.OnLoginEvent += GetPlayerLevel;
            // LevelManager.OnEnterGame += GetPlayerScore;
            NetworkManager.OnLogoutEvent += Logout;
            // NetworkManager.OnLoginEvent += GetBoosterData;
#elif EPSILON
            NetworkManager.OnLoginEvent += DownloadPlayerData;
                        levels = new List<EpsilonLevel>();
#endif

            LatestReachedLevel = 0;
        }

        public void Logout()
        {
            dataManager.Logout();
#if EPSILON
            NetworkManager.OnLoginEvent -= DownloadPlayerData;
#elif PLAYFAB || GAMESPARKS
            NetworkManager.OnLoginEvent -= GetPlayerLevel;
            // LevelManager.OnEnterGame -= GetPlayerScore;
            // NetworkManager.OnLoginEvent -= GetBoosterData;
            NetworkManager.OnLogoutEvent -= Logout;
#endif
        }

        #region SCORE

        public void SendLevelsUpdate()
        {
#if EPSILON
            dataManager.SetLevels();
#endif
        }

#if PLAYFAB || GAMESPARKS 
        public void SetPlayerScoreTotal()
        {//2.1.6
            int latestLevel = LevelsMap.GetLastestReachedLevel();
            for (int i = 1; i <= latestLevel; i++)
            {
                SetPlayerScore(i, PlayerPrefs.GetInt("Score" + i, 0));
            }
        }

        public void SetPlayerScore(int level, int score)
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            if (score <= LevelScoreCurrentRecord)
                return;

            dataManager.SetPlayerScore(level, score);
        }

        public void GetPlayerScore(int level)
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            dataManager.GetPlayerScore(level, value =>
            {
                LevelScoreCurrentRecord = value;
                PlayerPrefs.SetInt("Score" + level, LevelScoreCurrentRecord);
                PlayerPrefs.Save();
            });
        }
#endif

        #endregion

        #region LEVEL

#if PLAYFAB || GAMESPARKS
        public void SetPlayerLevel(int level)
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            if (level <= LatestReachedLevel)
                return;

            dataManager.SetPlayerLevel(level);
        }

#elif EPSILON
        public void SetPlayerLevel(EpsilonLevel level)
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            //if (level.level <= LatestReachedLevel)
            //    return;

            ((EpsilonDataManager) dataManager).SetLevel(level);
        }
#endif

#if EPSILON
        public void DownloadPlayerData()
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            // check saved data (testing purpose only)
            //CheckPrefData();
            
            dataManager.GetLevels(value =>
            {
                int d = 0;
                d++;

                int lastMapLevel = LevelsMap._mapProgressManager.GetLastLevel();
                List<EpsilonLevel> clientLevels = new List<EpsilonLevel>(lastMapLevel);
                for (int i = 1; i <= lastMapLevel; i++)
                {
                    int level = i;
                    int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", i), 0);
                    int score = PlayerPrefs.GetInt("Score" + i, 0); 
                    //Debug.Log("<color=yellow> lvl = </color>" + level + "    <= Star => " + stars + "    <= Score =>" + score);
                    clientLevels.Add(new EpsilonLevel(level, stars, score));
                }
                
                List<EpsilonLevel> clientLevelsToUpdate = prepareUpdateLevels(levels, clientLevels);
                if (clientLevelsToUpdate.Count() > 0 /*LevelsMap._instance.GetLastestReachedLevel() > LatestReachedLevel + 1*/)
                {
                    ((EpsilonDataManager) dataManager).SetLevels(clientLevelsToUpdate);
                    return;
                }


                PlayerPrefs.Save();
                if (LevelsMap._instance != null) LevelsMap._instance.Reset();
            });

            dataManager.GetBoosterData(serverBoosts =>
            {
                /*Dictionary<string, int> boostsToUpdate = new Dictionary<string, int>();

                string[] boostNames = new string[] {"currency1", "1", "2", "3", "4", "5", "6", "7" };

                for (int i = 0; i < boostNames.Length; i++)
                {
                    if(!dic.ContainsKey(boostNames[i]))
                    {
                        boostsToUpdate.Add(boostNames[i], 0);
                    }
                }

                if(boostsToUpdate.Count() > 0)
                {
                    dataManager.SetBoosterData(boostsToUpdate);
                }*/

                foreach (var srvBoost in serverBoosts)
                {
                    PlayerPrefs.SetInt("Boost_" + srvBoost.name, srvBoost.count);
                }

                PlayerPrefs.Save();
            });
        }


        public void CheckPrefData()
        {
            for (int i = 0; i <  LevelsMap._mapProgressManager.GetLastLevel(); i++)
            {
                Debug.Log("Level >>>  "+ i + "  >>>>>  " + 
                          PlayerPrefs.GetInt($"Level.{i:000}.StarsCount", 0) +"  <<<< >>>>>  " + 
                          PlayerPrefs.GetInt("Score" + i));   
            }
        }

        public List<EpsilonLevel> prepareUpdateLevels(List<EpsilonLevel> inputLevels, List<EpsilonLevel> clientLevels)
        {
            if (clientLevels == null || clientLevels.Count() <= 0) return new List<EpsilonLevel>();
            if (inputLevels == null || inputLevels.Count() <= 0) return clientLevels;

            List<EpsilonLevel> result = new List<EpsilonLevel>();

            foreach (EpsilonLevel clientLevel in clientLevels)
            {
                EpsilonLevel inputLevel = inputLevels.Find(e => e.level == clientLevel.level);
                if (inputLevel == null || clientLevel.IsGreatThen(inputLevel))
                    result.Add(clientLevel);
            }

            return result;
        }

        public static void setLevels(ResultObject[] inputLevels)
        {
            foreach (ResultObject lvl in inputLevels)
            {
                levels.Add(new EpsilonLevel(lvl.level, lvl.stars, lvl.score));

                PlayerPrefs.SetInt("OpenLevel", lvl.level);
                PlayerPrefs.SetInt($"Level.{lvl.level:000}.StarsCount", lvl.stars);
                PlayerPrefs.SetInt("Score" + lvl.level, lvl.score);
            }

            PlayerPrefs.Save();
            if (LevelsMap._instance != null) LevelsMap._instance.Reset();
        }
#endif

        private int GetMaxLevel()
        {
            int max = 0;

            return max;
        }
        #endregion
        
#if GAMESPARKS || PLAYFAB
        public void GetPlayerLevel()
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            dataManager.GetPlayerLevel(value => //2.1.5 Fixed: progress not saved after login
            {
                LatestReachedLevel = value;
                if (LatestReachedLevel <= 0)
                    NetworkManager.dataManager.SetPlayerLevel(1);
                GetStars();
            });
        }

        #region STARS

        public void SetStars(int level)
        {
            int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", level));
            dataManager.SetStars(stars, level);
        }

        public void GetStars()
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            Debug.Log(LevelsMap.GetLastestReachedLevel() + " " + LatestReachedLevel);
            if (LevelsMap.GetLastestReachedLevel() > LatestReachedLevel)
            {
                Debug.Log("reached higher level than synced");
                SyncAllData();
                return;
            }

            dataManager.GetStars(dic =>
            {
                foreach (var item in dic)
                {
                    PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", int.Parse(item.Key.Replace("StarsLevel_", ""))), item.Value);
                }
                PlayerPrefs.Save();
                LevelsMap._instance.Reset();

            });
        }
          #endregion
#endif

      

        //  #region BOOSTS

        public void sendUpdateBoost(string name, int newCount)
        {
#if EPSILON
            dataManager.UpdateBoost(name, newCount);
#endif
        }

        /*public void sendBoostsUpdate()
       {
           Dictionary<string, int> dic = new Dictionary<string, int>
           {
               {"Boost_" + (int) BoostType.ExtraMoves, PlayerPrefs.GetInt("" + BoostType.ExtraMoves)},
               {"Boost_" + (int) BoostType.Packages, PlayerPrefs.GetInt("" + BoostType.Packages)},
               {"Boost_" + (int) BoostType.Stripes, PlayerPrefs.GetInt("" + BoostType.Stripes)},
               {"Boost_" + (int) BoostType.ExtraTime, PlayerPrefs.GetInt("" + BoostType.ExtraTime)},
               {"Boost_" + (int) BoostType.Bomb, PlayerPrefs.GetInt("" + BoostType.Bomb)},
               {"Boost_" + (int) BoostType.ExplodeArea, PlayerPrefs.GetInt("" + BoostType.ExplodeArea)},
               {"Boost_" + (int) BoostType.FreeMove, PlayerPrefs.GetInt("" + BoostType.FreeMove)},
               {"Boost_" + (int) BoostType.MulticolorCandy, PlayerPrefs.GetInt("" + BoostType.MulticolorCandy)}
           };

           BoostsUpdateRequest boostsUpdateRequest = new BoostsUpdateRequest();

           foreach (var boost in dic)
           {
               boostsUpdateRequest.addBoost(new BoostsUpdateRequest.Boost(boost.Key, boost.Value));
           }

           EpsilonClientAPI.THIS.SendApi("game", "boosts", boostsUpdateRequest.toJson(), (response) =>
           {
               Debug.Log("Boosts update response: " + response.downloadHandler.text);
           });
       }*/

#if GAMESPARKS || PLAYFAB
        #region BOOSTS

        // public void SetBoosterData()
        // {
        //     Dictionary<string, string> dic = new Dictionary<string, string>
        //     {
        //         {"Boost_" + (int) BoostType.ExtraMoves, "" + PlayerPrefs.GetInt("" + BoostType.ExtraMoves)},
        //         {"Boost_" + (int) BoostType.Packages, "" + PlayerPrefs.GetInt("" + BoostType.Packages)},
        //         {"Boost_" + (int) BoostType.Stripes, "" + PlayerPrefs.GetInt("" + BoostType.Stripes)},
        //         {"Boost_" + (int) BoostType.ExtraTime, "" + PlayerPrefs.GetInt("" + BoostType.ExtraTime)},
        //         {"Boost_" + (int) BoostType.Bomb, "" + PlayerPrefs.GetInt("" + BoostType.Bomb)},
        //         {"Boost_" + (int) BoostType.ExplodeArea, "" + PlayerPrefs.GetInt("" + BoostType.ExplodeArea)},
        //         {"Boost_" + (int) BoostType.FreeMove, "" + PlayerPrefs.GetInt("" + BoostType.FreeMove)},
        //         {"Boost_" + (int) BoostType.MulticolorCandy, "" + PlayerPrefs.GetInt("" + BoostType.MulticolorCandy)}
        //     };
        //
        //     dataManager.SetBoosterData(dic);
        // }

        // public void GetBoosterData()
        // {
        //     if (!NetworkManager.THIS.IsLoggedIn)
        //         return;
        //
        //     dataManager.GetBoosterData(dic =>
        //     {
        //         foreach (var item in dic)
        //         {
        //             PlayerPrefs.SetInt("" + (BoostType)int.Parse(item.Key.Replace("Boost_", "")), item.Value);
        //         }
        //         PlayerPrefs.Save();
        //     });
        // }

        #endregion

        public void SetTotalStars()
        {
            LevelsMap.GetMapLevels().Where(l => !l.IsLocked).ToList().ForEach(i => dataManager.SetStars(i.StarsCount, i.Number)); //2.1.5
        }

        public void SyncAllData()
        {
            SetTotalStars();
            SetPlayerLevel(LevelsMap.GetLastestReachedLevel());
            // SetBoosterData();//2.1.5 sync boosters
            SetPlayerScoreTotal();//2.1.6 sync levels
            NetworkManager.currencyManager.SetBalance(PlayerPrefs.GetInt("Gems"));//2.1.5 sync currency

        }

#endif
    }
}

#endif