using SweetSugar.Scripts.Integrations.Network;
using UnityEngine;
#if FACEBOOK
using Facebook.Unity;

#endif

namespace SweetSugar.Scripts.GUI
{
	/// <summary>
	/// Hides or shows Facebook login button
	/// </summary>
	public class FBButton : MonoBehaviour {
		public bool showIfLogged;
		public GameObject button;
#if FACEBOOK
		void OnEnable () {
			if (button == null)
				button = gameObject;
#if PLAYFAB || GAMESPARKS || EPSILON
			NetworkManager.OnLoginEvent += Login;
			NetworkManager.OnLogoutEvent += LogOut;
#endif
			SwitchButton ();
		}

		void OnDisable () {
#if PLAYFAB || GAMESPARKS || EPSILON
			NetworkManager.OnLoginEvent -= Login;
			NetworkManager.OnLogoutEvent -= LogOut;
#endif
		}

		void SwitchButton () {
			if (FB.IsLoggedIn)
				button.SetActive (showIfLogged);
			else
				button.SetActive (!showIfLogged);
		
		}

		void Login () {
			SwitchButton ();
		}

		void LogOut () {
			SwitchButton ();
		}
#else
	void OnEnable () {
		gameObject.SetActive(false);
	}
#endif

	}
}
