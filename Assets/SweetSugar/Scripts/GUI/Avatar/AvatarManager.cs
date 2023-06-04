using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Integrations;
using SweetSugar.Scripts.Integrations.Network;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.Avatar
{
	/// <summary>
	/// Handles friend avatars on the map
	/// </summary>
	public class AvatarManager : MonoBehaviour {
		[SerializeField] private GameObject avatarPrefab;
		public List<GameObject> avatars = new List<GameObject> ();

		void OnEnable () {//1.3.3
#if PLAYFAB || GAMESPARKS || EPSILON
			NetworkManager.OnFriendsOnMapLoaded += CheckFriendsList;

			NetworkManager.friendsManager?.PlaceFriendsPositionsOnMap();
#endif
		}

		void OnDisable () {//1.3.3
#if PLAYFAB || GAMESPARKS || EPSILON
			NetworkManager.OnFriendsOnMapLoaded -= CheckFriendsList;
#endif
		}

		void CheckFriendsList () {
#if GAMESPARKS || PLAYFAB
            var Friends = FacebookManager.THIS.Friends;

            for (var i = 0; i < Friends.Count; i++)
            {
                CreateAvatar(Friends[i]);
            }
#elif EPSILON
			var Friends = FacebookManager.THIS.Friends.Where(i => i.id != FacebookManager.userID).ToArray();

			for (var i = 0; i < Friends.Length; i++)
			{
				CreateAvatar(Friends[i]);
			}
#endif
		}

		/// <summary>
		/// Creates the friend's avatar.
		/// </summary>
		void CreateAvatar (FriendData friendData) {
			var friendAvatar = friendData.avatar;
			if (friendAvatar == null) {
				friendAvatar = Instantiate (Resources.Load ("Prefabs/FriendAvatar")) as GameObject;
				avatars.Add (friendAvatar);
				friendData.avatar = friendAvatar;
				friendAvatar.transform.SetParent (transform);
			}
			friendAvatar.GetComponent<FriendAvatar> ().FriendData = friendData;
		}

	}
}
