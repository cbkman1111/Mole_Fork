using UnityEngine;
using UnityEngine.UI;

namespace LayerLab.ArtMaker
{

    public enum ButtonType
    {
        SavePrefab,
        LinkAssetStore,
        LinkDiscord,
        LinkFacebook,
        LinkYoutube,
        LinkAsset,
        PlayRandomParts,
        PlayHome,
        PlayExperience,
    }

    public class ButtonAction : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private ButtonType buttonType;


        #region Constants

        private const string PathDiscord = "https://discord.gg/qCsVSHHcY7";
        private const string PathFacebook = "https://www.facebook.com/layerlab";
        private const string PathYoutube = "https://www.youtube.com/@LayerlabGames";
        private const string PathAssetStore = "https://assetstore.unity.com/publishers/5232";

        #endregion

        private void OnValidate()
        {
            button ??= GetComponent<Button>();
        }

        void Start()
        {
            button.onClick.AddListener(() =>
            {
                switch (buttonType)
                {
                    case ButtonType.PlayHome:
                        DemoControl.Instance.OnClick_Home();
                        break;
                    case ButtonType.SavePrefab:
                        CharacterPrefabUtility.Instance.CreateCharacterPrefab();
                        break;
                    case ButtonType.LinkAssetStore:
                        AudioManager.Instance.PlaySound(SoundList.ButtonDefault);
                        Application.OpenURL(PathAssetStore);
                        break;
                    case ButtonType.LinkAsset:
                        AudioManager.Instance.PlaySound(SoundList.ButtonDefault);
                        Application.OpenURL(DemoControl.Instance.pathAsset);
                        break;
                    case ButtonType.LinkDiscord:
                        AudioManager.Instance.PlaySound(SoundList.ButtonDefault);
                        Application.OpenURL(PathDiscord);
                        break;
                    case ButtonType.LinkFacebook:
                        AudioManager.Instance.PlaySound(SoundList.ButtonDefault);
                        Application.OpenURL(PathFacebook);
                        break;
                    case ButtonType.LinkYoutube:
                        AudioManager.Instance.PlaySound(SoundList.ButtonDefault);
                        Application.OpenURL(PathYoutube);
                        break;
                    case ButtonType.PlayRandomParts:
                        DemoControl.Instance.OnClick_RandomParts();
                        break;
                    case ButtonType.PlayExperience:
                        DemoControl.Instance.OnClick_Experience();
                        break;

                }
            });
        }
    }
}