using System;
#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif

namespace SweetSugar.Scripts.Integrations.Network.PlayFab
{
	public class PlayFabCurrencyManager : ICurrencyManager {

		public PlayFabCurrencyManager () {
		}


		public  void IncBalance (int amount) {
			#if PLAYFAB
		PlayFab.ClientModels.AddUserVirtualCurrencyRequest request = new PlayFab.ClientModels.AddUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC",
			Amount = amount
		};

		PlayFabClientAPI.AddUserVirtualCurrency (request, (result) => {
			Debug.Log (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});
			#endif
		}

		public  void DecBalance (int amount) {
			#if PLAYFAB
		PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest request = new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC",
			Amount = amount
		};

		PlayFabClientAPI.SubtractUserVirtualCurrency (request, (result) => {
			Debug.Log (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});
			#endif
		}

		public  void SetBalance (int newbalance) {
			#if PLAYFAB
		PlayFab.ClientModels.AddUserVirtualCurrencyRequest request = new PlayFab.ClientModels.AddUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC",
			Amount = newbalance - NetworkCurrencyManager.currentBalance
		};

		PlayFabClientAPI.AddUserVirtualCurrency (request, (result) => {
			Debug.Log (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});

			#endif
		}

		public  void GetBalance (Action<int> Callback) {
			#if PLAYFAB
		PlayFab.ClientModels.AddUserVirtualCurrencyRequest request = new PlayFab.ClientModels.AddUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC"
		};

		PlayFabClientAPI.AddUserVirtualCurrency (request, (result) => {
			Callback (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
				//GetCurrencyList();
			});

			#endif
		}

	}
}

