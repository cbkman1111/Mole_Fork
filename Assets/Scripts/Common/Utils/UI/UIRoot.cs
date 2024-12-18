using Common.Global;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.UIObject
{
    public class UIRoot : MonoBehaviour
    {
        public CanvasGroup _canvasMain;
        public CanvasController _controllerMenu;
        public CanvasController _controllerHud;
        public CanvasController _controllerPopup;
        public CanvasController _controllerEtc;
        public Transform _cover;

        [SerializeField] 
        private Dictionary<string, string> pathInfo = new();

        private void Awake()
        {
            _cover.gameObject.SetActive(false);
        }

        public void AddPath(string key, string path)
        {
            pathInfo.TryAdd(key, path);
        }
    }
}
