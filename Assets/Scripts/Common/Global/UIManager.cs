using System;
using System.Collections.Generic;
using System.Linq;
using Common.Global.Singleton;
using Common.UIObject;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.Global
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private CanvasGroup _canvasMain;
        private CanvasController _controllerMenu;
        private CanvasController _controllerHud;
        private CanvasController _controllerPopup;
        private CanvasController _controllerEtc;
        private Transform _cover;

        // ReSharper disable Unity.PerformanceAnalysis
        protected override bool Init()
        {
            const string uiRoot = "UI/UIRoot";
            var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>(uiRoot);
            var root = Instantiate(prefab, transform);
            if(root == false)
            {
                Debug.LogError($"root is null.");
                return false;
            }

            root.name = "UIRoot";
            root.transform.position = new Vector3(100,0,0);

            _canvasMain = root.GetComponent<CanvasGroup>();
        
            _controllerMenu = new CanvasController();
            _controllerMenu.Init(root.transform.Find("Canvas - menu"));
            _controllerHud = new CanvasController();
            _controllerHud.Init(root.transform.Find("Canvas - hud"));
            _controllerPopup = new CanvasController();
            _controllerPopup.Init(root.transform.Find("Canvas - popup"));
            _controllerEtc = new CanvasController();
            _controllerEtc.Init(root.transform.Find("Canvas - etc"));

            _cover = root.transform.Find("Image - dim");
            _cover.gameObject.SetActive(false);

            gameObject.name = $"singleton - {Tag}";
            return true;
        }

        public T OpenMenu<T>(string menuName) where T : MenuBase
        {
            var ret = _controllerMenu.Open<T>(menuName);
            if (ret != null)
            {
                ret.OnInit();
            }
        
            return ret;
        }

        public T OpenHud<T>(string hudName) where T : HudBase
        {
            var ret = _controllerPopup.Open<T>(hudName);
            if (ret == true)
            {
                ret.OnInit();
            }

            return ret;
        }

        public T OpenPopup<T>(string popupName) where T : PopupBase
        {
            var ret = _controllerPopup.Open<T>(popupName);
            if (ret == true)
            {
                ret.OnInit();
            }

            CoverCheck();
            return ret;
        }

        public T OpenEtc<T>(string etcName) where T : UIObject.UIObject
        {
            var ret = _controllerPopup.Open<T>(etcName);
            if (ret == true)
            {
                ret.OnInit();
            }

            CoverCheck();
            return ret;
        }

        public bool FindPopup(string popupName)
        {
            return _controllerPopup.Get(popupName) == true;
        }

        public void CloseMenu(string menuName)
        {
            _controllerMenu.Close(menuName);
        }

        public void CloseHud(string hudName)
        {
            _controllerHud.Close(hudName);
        }

        public void ClosePopup(string popupName)
        {
            _controllerPopup.Close(popupName);
            CoverCheck();
        }

        public void CloseEtc(string etcName)
        {
            _controllerEtc.Close(etcName);
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

        public void Clear()
        {
            _controllerMenu.Clear();
            _controllerHud.Clear();
            _controllerPopup.Clear();
            _controllerEtc.Clear();
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

    public class CanvasController
    {
        private Canvas _canvas;
        private readonly List<UIObject.UIObject> _list = new List<UIObject.UIObject>();

        public void Init(Transform trans)
        {
            _canvas = trans.GetComponent<Canvas>();
        }

        public Transform GetTransform()
        {
            return _canvas.transform;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public T Open<T>(string name) where T : UIObject.UIObject
        {
            var trans = _canvas.transform.Find(name);
            T ret;
        
            if(trans == false)
            {
#if UNITY_EDITOR
                var prefab = ResourcesManager.Instance.LoadInBuild<T>(name); 
#else
            T prefab = ResourcesManager.Instance.LoadBundle<T>(name);
            if (prefab == null)
            {
                prefab = ResourcesManager.Instance.LoadInBuild<T>(name);
            }
#endif
                if (prefab == false)
                { 
                    return default(T); 
                }

                var clone = Object.Instantiate(prefab, _canvas.transform);
                if (clone == false)
                {
                    Debug.LogError($"clone is null.");
                    return default(T);
                }

                clone.transform.SetAsLastSibling();
                clone.name = name;
                ret = clone;
            }
            else
            {
                ret = trans.GetComponent<T>();
            }

            if(_list.Contains(ret) == false)
            {
                _list.Add(ret);
            }

            return ret;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void Close(string name)
        {
            var trans = _canvas.transform.Find(name);
            if (trans != true) return;
            var obj = trans.GetComponent<UIObject.UIObject>();
            obj.OnClose();

            if (_list != null && _list.Contains(obj))
            {
                _list.Remove(obj);
            }

            Object.Destroy(trans.gameObject);
        }

        public void Close(Transform transform)
        {
            Close(transform.name);
        }

        public Transform Get(string name)
        {
            var ret = _list.Where(obj => string.Compare(obj.name, name, StringComparison.Ordinal) == 0).ToList();
            return ret.Count == 1 ? ret[0].transform : null;
        }

        public T Get<T>(string name) where T : UIObject.UIObject
        {
            var obj = _canvas.transform.Find(name);
            return obj.GetComponent<T>();
        }

        public int Count()
        {
            return _list.Count;
        }

        public Transform Last()
        {
            var index = _canvas.transform.childCount - 1;
            Transform trans = null;
            if(index >= 0)
            {
                trans = _canvas.transform.GetChild(index);
            }
        
            return trans;
        }

        public void Clear()
        {
            for (var i = 0; i < _canvas.transform.childCount ;i++)
            {
                var trans = _canvas.transform.GetChild(0);
                Object.Destroy(trans.gameObject);
            }
        }
    }
}