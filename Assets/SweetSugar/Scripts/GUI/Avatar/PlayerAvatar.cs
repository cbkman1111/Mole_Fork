
using System.Linq;
using SweetSugar.Scripts.Integrations.Network;
using SweetSugar.Scripts.MapScripts;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI.Avatar
{
    /// <summary>
    /// Player avatar. Loading picture and restore it after back to map scene
    /// </summary>
    public class PlayerAvatar : MonoBehaviour, IAvatarLoader
    {

        public Image image;
#if PLAYFAB || GAMESPARKS || EPSILON

        void Start()
        {
            image.enabled = false;
        }

        void OnEnable () {
            LevelsMap.LevelReached += OnLevelReached;
#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.OnPlayerPictureLoaded += ShowPicture;
            if(NetworkManager.profilePic != null) ShowPicture();
#endif
        }

        void OnDisable () {
#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.OnPlayerPictureLoaded -= ShowPicture;
            LevelsMap.LevelReached -= OnLevelReached;
#endif

        }
#endif

        public void ShowPicture()
        {
#if PLAYFAB || GAMESPARKS || EPSILON
            image.sprite = NetworkManager.profilePic;
#endif
            image.enabled = true;
        }

        private void OnLevelReached(object sender, LevelReachedEventArgs e)
        {
            Debug.Log(string.Format("Level {0} reached.", e.Number));
        }
    }
}
