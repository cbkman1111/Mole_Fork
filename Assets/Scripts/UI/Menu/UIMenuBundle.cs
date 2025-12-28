using System.Collections.Generic;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuBundle : MenuBase
    {
        private SpriteAtlas atlas = null;
        [SerializeField]
        private Image test = null;

        public bool InitMenu()
        {
            SetTextMeshPro("Text (TMP) - DownloadSize", $"{0} - bytes");
            SetTextMeshPro("Text (TMP) - Desctiption", $"Category - {0}");
            return true;
        }

        private IEnumerator<float> DownloadDependenciesAsync()
        {
            yield return MEC.Timing.WaitForOneFrame;

            string key = "default";
            var asyncHandle = Addressables.DownloadDependenciesAsync(key);
            asyncHandle.Completed += (handle) => {
                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        var percent = asyncHandle.GetDownloadStatus().Percent * 100.0f;
                        SetTextMeshPro("Text (TMP) - DownloadSize", $"Downloaded : {dowonloadSize} / {percent}%");
                        break;
                    case AsyncOperationStatus.Failed:
                        SetTextMeshPro("Text (TMP) - DownloadSize", $"Download failed : {handle.OperationException}");
                        break;
                    case AsyncOperationStatus.None:
                        SetTextMeshPro("Text (TMP) - DownloadSize", $"Download none");
                        break;
                }
            };

            while(!asyncHandle.IsDone)
            {
                var percent = asyncHandle.GetDownloadStatus().Percent * 100.0f;
                SetTextMeshPro("Text (TMP) - DownloadSize", $"Downloading : {dowonloadSize} / {percent}%");
                yield return MEC.Timing.WaitForOneFrame;
            }

            Addressables.Release(asyncHandle);
        }

        private long dowonloadSize = 0;
        private IEnumerator<float> GetDownloadSizeAsync()
        {
            yield return MEC.Timing.WaitForOneFrame;
            string key = "default";

            var asyncHandle = Addressables.GetDownloadSizeAsync(key);
            dowonloadSize = asyncHandle.Result;
            string sizeText = string.Concat(dowonloadSize, " bytes");
            SetTextMeshPro("Text (TMP) - DownloadSize", $"{sizeText}");

            Addressables.Release(asyncHandle);
        }

        /// <summary>
        /// CheckForCatalogUpdates()의 반환값은 내부적으로 Addressables 시스템이 관리하는 핸들입니다.
        /// 이 핸들은 Addressables가 자동으로 해제(Release)할 수 있으므로,
        /// 사용자가 명시적으로 Release를 호출하면 중복 해제가 되어 예외가 발생할 수 있습니다.
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> CheckForCatalogUpdates()
        {
            yield return MEC.Timing.WaitForOneFrame;

            var asyncHandle = Addressables.CheckForCatalogUpdates();
            asyncHandle.Completed += (handle) =>
            {
                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        SetTextMeshPro("Text (TMP) - Desctiption", $"Success : catalog count : {asyncHandle.Result.Count}");
                        break;
                    case AsyncOperationStatus.Failed:
                        SetTextMeshPro("Text (TMP) - Desctiption", $"{asyncHandle.OperationException}");
                        break;
                    case AsyncOperationStatus.None:
                        SetTextMeshPro("Text (TMP) - Desctiption", $"none");
                        break;
                }

                //Addressables.CheckForCatalogUpdates()의 결과로 받은 asyncHandle을 Completed 콜백에서
                // Addressables.Release(asyncHandle);로 해제하면,
                // Unity Addressables 1.22.x 버전에서 내부적으로 이미 해제된 핸들을 다시 해제하려고 할 때
                // "Attempting to use an invalid operation handle" 예외가 발생할 수 있습니다
                //Addressables.Release(asyncHandle);
            };
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneMenu);
            }
            else if (name == "Button - CheckForCatalogUpdates")
            {
                MEC.Timing.RunCoroutine(CheckForCatalogUpdates());
            }
            else if (name == "Button - GetDownloadSizeAsync")
            {
                MEC.Timing.RunCoroutine(GetDownloadSizeAsync());
            }
            else if (name == "Button - DownloadDependenciesAsync")
            {
                MEC.Timing.RunCoroutine(DownloadDependenciesAsync());
            }
            else if (name == "Button - ClearDependencyCacheAsync")
            {
                Addressables.ClearDependencyCacheAsync("default");
            }
            else if (name == "Button - LoadAssetAsync")
            {
                var asyncHandle = Addressables.LoadAssetAsync<SpriteAtlas>("TestAtlas");
                asyncHandle.Completed += handle =>
                {
                    switch (asyncHandle.Status)
                    {
                        case AsyncOperationStatus.Succeeded:
                            atlas = asyncHandle.Result;
                            //test.sprite = sprite;
                            //test.SetNativeSize();

                            var sprite = atlas.GetSprite("youtube");
                            test.sprite = sprite;
                            break;

                        case AsyncOperationStatus.Failed:
                            break;

                        case AsyncOperationStatus.None:
                            break;
                    }
                };
            }
        }
    }
}
