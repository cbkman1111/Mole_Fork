using System;
using System.Collections.Generic;
#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif

namespace SweetSugar.Scripts.Integrations.Network.PlayFab
{
	public class PlayFabDataManager : IDataManager {

		public	void Logout () {//1.3.3
		}


		public  void SetData (Dictionary<string, string> dic) {
			#if PLAYFAB
		if (!NetworkManager.THIS.IsLoggedIn)
			return;

		UpdateUserDataRequest request = new UpdateUserDataRequest () {
			Data = dic
		};

		PlayFabClientAPI.UpdateUserData (request, (result) => {
			Debug.Log ("Successfully updated user data");
		}, (error) => {
			Debug.Log (error.ErrorDetails);
		});
			#endif

		}


		#region SCORE

		//	public static void SetPlayerScoreTotal () {//1.3.3
		//		int latestLevel = LevelsMap._instance.GetLastestReachedLevel ();
		//		for (int i = 1; i <= latestLevel; i++) {
		//			SetPlayerScore (i, PlayerPrefs.GetInt ("Score" + i, 0));//TODO sync Level_
		//		}
		//	}

		public  void SetPlayerScore (int level, int score) {
			#if PLAYFAB
		UpdatePlayerScoreFoLeadboard (score);

		List<StatisticUpdate> stUpdateList = new List<StatisticUpdate> ();
		StatisticUpdate stUpd = new StatisticUpdate ();
		stUpd.StatisticName = "Level_" + level;
		stUpd.Value = score;
		stUpdateList.Add (stUpd);

		UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest () {
			Statistics = stUpdateList
		};

		PlayFabClientAPI.UpdatePlayerStatistics (request, (result) => {
			Debug.Log ("Successfully updated user score");
		}, (error) => {
			Debug.Log (error.ErrorDetails);
		});
			#endif
		}

		public  void GetPlayerScore (Action<int> Callback) {
			#if PLAYFAB
		GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest () {
			StatisticNames = new List<string> () { "Level_" + LevelManager.THIS.currentLevel }
		};
				
		PlayFabClientAPI.GetPlayerStatistics (request, (result) => {
			if ((result.Statistics == null)) {
				Debug.Log ("No user data available");
			} else {
				foreach (var item in result.Statistics) {
					if (item.StatisticName == "Level_" + LevelManager.THIS.currentLevel) {
						Callback (item.Value);
					}
//					Debug.Log ("    " + item.StatisticName + " == " + item.Value);
				}
			}
		}, (error) => {
			Debug.Log (error.ErrorDetails);
		});


			#endif
		}
	
		public void GetPlayerScore(int level, Action<int> Callback) {
			#if PLAYFAB
		GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest () {
			StatisticNames = new List<string> () { "Level_" + level }
		};
				
		PlayFabClientAPI.GetPlayerStatistics (request, (result) => {
			if ((result.Statistics == null)) {
				Debug.Log ("No user data available");
			} else {
				foreach (var item in result.Statistics) {
					if (item.StatisticName == "Level_" + level) {
						Callback (item.Value);
					}
//					Debug.Log ("    " + item.StatisticName + " == " + item.Value);
				}
			}
		}, (error) => {
			Debug.Log (error.ErrorDetails);
		});


			#endif
		}

		void UpdatePlayerScoreFoLeadboard (int score) {
			#if PLAYFAB
		LeadboardPlayerData leadboardPlayerData = NetworkManager.THIS.leadboardList.Find (delegate (LeadboardPlayerData bk) {
			return bk.userID == NetworkManager.THIS.UserID;
		}
		                                          );
		if (leadboardPlayerData != null)
			leadboardPlayerData.score = score;
			#endif
		}


		#endregion


		#region LEVEL

		public  void SetPlayerLevel (int level) {
			#if PLAYFAB
		NetworkDataManager.LatestReachedLevel = level;//1.3.3
		List<StatisticUpdate> stUpdateList = new List<StatisticUpdate> ();
		StatisticUpdate stUpd = new StatisticUpdate ();
		stUpd.StatisticName = "Level";
		stUpd.Value = level;
		stUpdateList.Add (stUpd);

		UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest () {
			Statistics = stUpdateList
		};

		PlayFabClientAPI.UpdatePlayerStatistics (request, (result) => {
			Debug.Log ("Successfully updated user level");
		}, (error) => {
			Debug.Log (error.ErrorDetails);
		});
			#endif
		}

		public  void GetPlayerLevel (Action<int> Callback) {
			#if PLAYFAB
		GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest () {
			StatisticNames = new List<string> () { "Level" }
		};

		PlayFabClientAPI.GetPlayerStatistics (request, (result) => {
			if ((result.Statistics == null)) {
				Debug.Log ("No user data available");
			} else {
				foreach (var item in result.Statistics) {
					if (item.StatisticName == "Level")
						Callback (item.Value);
					//Debug.Log("    " + item.StatisticName + " == " + item.Value);
				}

			}
		}, (error) => {
			Debug.Log (error.ErrorDetails);
		});


			#endif
		}
	

		#endregion

		#region STARS

		public  void SetStars (int Stars, int Level) {
			#if PLAYFAB
		Dictionary<string, string> dic = new Dictionary<string, string> ();
//		for (int i = 1; i <= LevelManager.THIS.currentLevel; i++) {//1.3 
		dic.Add ("StarsLevel_" + Level, Stars.ToString ());
//		}
		SetData (dic);
			#endif
		}

		public  void SetTotalStars () {//1.3.3
			#if PLAYFAB
		Dictionary<string, string> dic = new Dictionary<string, string> ();
		int latestLevel = LevelsMap.GetLastestReachedLevel ();
		for (int i = 1; i <= latestLevel; i++) {
			dic.Add ("StarsLevel_" + i, "" + PlayerPrefs.GetInt (string.Format ("Level.{0:000}.StarsCount", i)));
		}
		SetData (dic);
			#endif
		}


		// Gets Stars by level and place player to the latest level
		public  void GetStars (Action<Dictionary<string,int>> Callback) {
			#if PLAYFAB

		string PlayFabId = NetworkManager.THIS.UserID;

		GetUserDataRequest request = new GetUserDataRequest () {
			PlayFabId = PlayFabId,
			Keys = null
		};

		PlayFabClientAPI.GetUserData (request, (result) => {
			if ((result.Data == null) || (result.Data.Count == 0)) {
				Debug.Log ("No user data available");
			} else {
				Dictionary<string,int> starsDic = new Dictionary<string, int> ();
				foreach (var item in result.Data) {
					if (item.Key.Contains ("StarsLevel_")) {
						//				Debug.Log (LatestReachedLevel);
						starsDic.Add (item.Key, int.Parse (item.Value.Value));
					}
				}
				Callback (starsDic);

			}
		}, (error) => {
			Debug.Log ("Got error retrieving user data:");
			Debug.Log (error.ErrorMessage);
		});
			#endif
		}

		#endregion


		#region BOOSTS

		public  void SetBoosterData (Dictionary<string, string> dic) {
			SetData (dic);
		}


		public  void GetBoosterData (Action<Dictionary<string,int>> Callback) {
			#if PLAYFAB
		string PlayFabId = NetworkManager.THIS.UserID;

		GetUserDataRequest request = new GetUserDataRequest () {
			PlayFabId = PlayFabId,
			Keys = null
		};

		PlayFabClientAPI.GetUserData (request, (result) => {
			if ((result.Data == null) || (result.Data.Count == 0)) {
				Debug.Log ("No user data available");
			} else {
				Dictionary<string,int> dicBoost = new Dictionary<string, int> ();
				foreach (var item in result.Data) {
					if (item.Key.Contains ("Boost_")) {
						dicBoost.Add (item.Key, int.Parse (item.Value.Value));
						//Debug.Log("    " + item.Key + " == " + item.Value.Value);
					}
				}
				Callback (dicBoost);
			}
		}, (error) => {
			Debug.Log ("Got error retrieving user data:");
			Debug.Log (error.ErrorMessage);
		});
			#endif
		}

		#endregion



	}
}

