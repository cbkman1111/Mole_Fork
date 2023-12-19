using Common.Global;
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

        private void Awake()
        {
            _cover.gameObject.SetActive(false);
        }
    }
}
