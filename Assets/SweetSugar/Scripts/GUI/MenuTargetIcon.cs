using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Target icon in the MenuPlay
    /// </summary>
    public class MenuTargetIcon : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI description;
        private Image[] images;

        private void Awake()
        {
            images = transform.GetChildren().Select(i => i.GetComponent<Image>()).ToArray();
        }

    void OnEnable()
    {
        DisableImages();
        var levelData = new LevelData(Application.isPlaying, LevelManager.THIS.currentLevel);
        levelData = LoadingManager.LoadForPlay(PlayerPrefs.GetInt("OpenLevel"), levelData);
        var list = levelData.GetTargetSprites();
        description.text = levelData.GetTargetContainersForUI().First().targetLevel.GetDescription();
        for (int i = 0; i < list.Length; i++)
        {
            images[i].sprite = list[i];
            images[i].gameObject.SetActive(true);
        }
    }

        private void DisableImages()
        {
            foreach (var item in images)
            {
                item.gameObject.SetActive(false);
            }
        }

    }
}
