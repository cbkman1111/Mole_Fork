using BehaviorDesigner.Runtime.Tasks;
using Common.Global.Singleton;
using Common.UIObject;
using Common.Utils;
using UnityEngine;


namespace Common.Global
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private UIRoot rootObject = null;
        
        private CanvasGroup _canvasMain { get => rootObject._canvasMain; }
        private CanvasController _controllerMenu { get => rootObject._controllerMenu; }
        private CanvasController _controllerHud { get => rootObject._controllerHud; }
        private CanvasController _controllerPopup { get => rootObject._controllerPopup; }
        private CanvasController _controllerEtc { get => rootObject._controllerEtc; }
        private Transform _cover { get => rootObject._cover; }

        // ReSharper disable Unity.PerformanceAnalysic
        protected override bool Init()
        {
            GiantDebug.Log($"{tag} - Init");

            const string uiRoot = "UI/UIRoot";
            var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>(uiRoot);
            var obj = Instantiate(prefab, transform);
            if(obj == false)
            {
                GiantDebug.LogError($"root is null.");
                return false;
            }

            obj.name = "UIRoot";
            obj.transform.position = new Vector3(100,0,0);
            rootObject = obj.GetComponent<UIRoot>();

            GiantDebug.Log($"{tag} - Init return true.");
            return true;
        }

        public T OpenMenu<T>() where T : MenuBase
        {
            string name = typeof(T).Name;
            var ret = _controllerMenu.Open<T>(name);
            if (ret != null)
            {
                ret.OnInit();
            }
        
            return ret;
        }

        public T OpenHud<T>() where T : HudBase
        {
            string name = typeof(T).Name;
            var ret = _controllerPopup.Open<T>(name);
            if (ret == true)
            {
                ret.OnInit();
            }

            return ret;
        }

        public T OpenPopup<T>() where T : PopupBase
        {
            string name = typeof(T).Name;
            var ret = _controllerPopup.Open<T>(name);
            if (ret == true)
            {
                ret.OnInit();
            }

            CoverCheck();
            return ret;
        }

        public T OpenEtc<T>() where T : UIObject.UIObject
        {
            string name = typeof(T).Name;
            var ret = _controllerPopup.Open<T>(name);
            if (ret == true)
            {
                ret.OnInit();
            }

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
            _controllerPopup.Close(name);
            CoverCheck();
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
                _cover.gameObject.SetActive(true);
            }
            else if(countPopup > 0)
            {
                _cover.SetParent(_controllerPopup.GetTransform());
                _cover.SetSiblingIndex(countPopup - 1);
                _cover.gameObject.SetActive(true);
            }
            else
            {
                _cover.SetParent(_canvasMain.transform);
                _cover.SetSiblingIndex(0);
                _cover.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// UI 모두 제거.
        /// </summary>
        public void Clear()
        {
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