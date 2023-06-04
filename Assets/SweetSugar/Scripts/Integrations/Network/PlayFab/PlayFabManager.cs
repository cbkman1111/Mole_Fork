#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif

//using PlayFab.AdminModels;

namespace SweetSugar.Scripts.Integrations.Network.PlayFab
{
	public class PlayFabManager : ILoginManager {
		public string PlayFabId;



		// Use this for initialization

		#region AUTHORIZATION

		public void LoginWithFB (string accessToken, string titleId) {
			#if PLAYFAB
		LoginWithFacebookRequest request = new LoginWithFacebookRequest () {
			TitleId = titleId,
			CreateAccount = true,
			AccessToken = accessToken
			//  CustomId = SystemInfo.deviceUniqueIdentifier
		};

		PlayFabClientAPI.LoginWithFacebook (request, (result) => {
			PlayFabId = result.PlayFabId;
			Debug.Log ("Got PlayFabID: " + PlayFabId);
			NetworkManager.THIS.UserID = PlayFabId; //TODO: think about login lambda
			if (result.NewlyCreated) {
				Debug.Log ("(new account)");
			} else {
				Debug.Log ("(existing account)");
			}
			NetworkManager.THIS.IsLoggedIn = true;
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});
			#endif
		}


		void Login (string titleId) {
			#if PLAYFAB
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest () {
			TitleId = titleId,
			CreateAccount = true,
			CustomId = SystemInfo.deviceUniqueIdentifier
		};

		PlayFabClientAPI.LoginWithCustomID (request, (result) => {
			PlayFabId = result.PlayFabId;
			Debug.Log ("Got PlayFabID: " + PlayFabId);

			if (result.NewlyCreated) {
				Debug.Log ("(new account)");
			} else {
				Debug.Log ("(existing account)");
			}
			NetworkManager.THIS.IsLoggedIn = true;
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});
			#endif
		}

		public void UpdateName (string userName) {
			#if PLAYFAB
		PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest request = new PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest () {
			DisplayName = userName
		};

		PlayFabClientAPI.UpdateUserTitleDisplayName (request, (result) => {
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});

			#endif
		}

		public bool IsYou (string playFabId) {
			#if PLAYFAB
		if (playFabId == PlayFabId)
			return true;
			#endif
			return false;
		}


		#endregion

	}
}

