using BehaviorDesigner.Runtime.Tasks;
using Common.Global.Singleton;
using Common.UIObject;
using Common.Utils;
using System;
using System.Linq;
using UnityEngine;


namespace Common.Global
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private UIRootDontDestroy rootDontDestroy = null;
        private CanvasController _controllerDontDestroy { get => rootDontDestroy._controller; }

        private UIRoot rootObject = null;
        private CanvasGroup _canvasMain { get => rootObject._canvasMain; }
        private CanvasController _controllerMenu { get => rootObject._controllerMenu; }
        private CanvasController _controllerHud { get => rootObject._controllerHud; }
        private CanvasController _controllerPopup { get => rootObject._controllerPopup; }
        private CanvasController _controllerEtc { get => rootObject._controllerEtc; }
        private Transform _cover { get => rootObject._cover; }

        /// <summary>
        /// 싱글턴 생성.
        /// </summary>
        /// <returns></returns>
        protected override bool Init()
        {
            const string address = "Assets/AddressableAssets/UI/UIRootDontDestroy.prefab";
            var prefab = ResourcesManager.Instance.LoadBundle(address);
            if (prefab == null)
            {
                return false;
            }

            var obj = Instantiate(prefab, transform);
            if (obj == false)
            {
                GiantDebug.LogError($"root is null.");
                return false;
            }

            obj.name = "UIRoot";
            obj.transform.position = new Vector3(100, 0, 0);
            rootDontDestroy = obj.GetComponent<UIRootDontDestroy>();
            GiantDebug.Log($"{tag} - Init");
            return true;
        }

        public bool InitWithScene(UnityEngine.SceneManagement.Scene scene)
        {
            const string uiRoot = "Assets/AddressableAssets/UI/UIRoot.prefab";
            var objs = scene.GetRootGameObjects();
            var root = objs.FirstOrDefault(obj => obj.name == "UIRoot");
            if (root == null)
            {
                var prefab = ResourcesManager.Instance.LoadBundle(uiRoot);
                var obj = Instantiate(prefab, null);
                if (obj == false)
                {
                    return false;
                }

                obj.name = "UIRoot";
                obj.transform.position = new Vector3(100, 0, 0);
                rootObject = obj.GetComponent<UIRoot>();
            }
            else
            {
                root.name = "UIRoot";
                root.transform.position = new Vector3(100, 0, 0);
                rootObject = root.GetComponent<UIRoot>();
            }

            return true;
        }

        /// <summary>
        /// Path 어트리뷰트에서 경로를 가져옵니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string GetPath<T>()
        {
            var attribute = (PathAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PathAttribute));
            return attribute.ResourcePath;
        }

        public T OpenDontDesroyPopup<T>() where T : PopupBase
        {
            var path = GetPath<T>();
            var name = typeof(T).Name;
            var ret = _controllerDontDestroy.Open<T>(path, name);
            return ret;
        }

        public T OpenMenu<T>() where T : MenuBase
        {
            var path = GetPath<T>();
            var name = typeof(T).Name;
            var ret = _controllerMenu.Open<T>(path, name);

            return ret;
        }

        public T OpenHud<T>() where T : HudBase
        {
            var path = GetPath<T>();
            var name = typeof(T).Name;
            var ret = _controllerPopup.Open<T>(path, name);

            return ret;
        }


        public T OpenPopup<T>() where T : PopupBase
        {
            var path = GetPath<T>();
            var name = typeof(T).Name;
            var ret = _controllerPopup.Open<T>(path, name);

            CoverCheck();
            return ret;
        }

        public T OpenEtc<T>() where T : UIObject.UIObject
        {
            var path = GetPath<T>();
            var name = typeof(T).Name;
            var ret = _controllerPopup.Open<T>(path, name);

            CoverCheck();
            return ret;
        }

        public bool FindPopup<T>()
        {
            string name = typeof(T).Name;
            return _controllerPopup.Get(name) == true;
        }

        public void CloseMenu<T>()
        {
            string name = typeof(T).Name;
            _controllerMenu.Close(name);
        }

        public void CloseMenu(string name)
        {
            _controllerMenu.Close(name);
        }

        public void CloseHud<T>()
        {
            string name = typeof(T).Name;
            _controllerHud.Close(name);
        }

        public void CloseHud(string name)
        {
            _controllerHud.Close(name);
        }

        public void ClosePopup<T>()
        {
            var name = typeof(T).Name;
            _controllerPopup.Close(name);
            CoverCheck();
        }

        public void CloseDontDestroyPopup<T>()
        {
            string name = typeof(T).Name;
            _controllerDontDestroy.Close(name);
        }

        public void ClosePopup(string name)
        {
            _controllerPopup.Close(name);
            CoverCheck();
        }

        public void CloseEtc<T>()
        {
            string name = typeof(T).Name;
            _controllerEtc.Close(name);
            CoverCheck();
        }

        private void CoverCheck()
        {
            var countEtc = _controllerEtc.Count();
            var countPopup = _controllerPopup.Count();

            if(countEtc > 0)
            {
                _cover.SetParent(_controllerEtc.GetTransform());
                _cover.SetSiblingIndex(countEtc - 1);
                _cover.SetActive(true);
            }
            else if(countPopup > 0)
            {
                _cover.SetParent(_controllerPopup.GetTransform());
                _cover.SetSiblingIndex(countPopup - 1);
                _cover.SetActive(true);
            }
            else
            {
                _cover.SetParent(_canvasMain.transform);
                _cover.SetSiblingIndex(0);
                _cover.SetActive(false);
            }
        }

        /// <summary>
        /// UI 모두 제거.
        /// </summary>
        public void Clear()
        {
            if (rootObject == null)
                return;

            _cover.SetParent(rootObject.transform);

            if (_controllerMenu != null)
            {
                _controllerMenu.Clear();
            }

            if (_controllerHud != null)
            {
                _controllerHud.Clear();
            }

            if (_controllerPopup != null)
            {
                _controllerPopup.Clear();
            }

            if (_controllerEtc != null)
            {
                _controllerEtc.Clear();
            }
        }

        public void BackKey()
        {
            if(_controllerEtc.Last() == true)
            {
                _controllerEtc.Close(_controllerEtc.Last());
            }
            else if (_controllerPopup.Last() == true)
            {
                _controllerPopup.Close(_controllerPopup.Last());
            }
            else
            {
                // 더이상 닫을 팝업이 없음.
            }

            CoverCheck();
        }
    }
}