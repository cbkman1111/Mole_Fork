using Common.Global;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UIObject
{
    //[Path("UI/Prefabs/Menu")]
    [Path("Assets/AddressableAssets/UI/Prefabs/Menu")]
    public abstract class MenuBase : UIObject
    {
        protected override void Awake()
        {
            base.Awake(); // 부모의 Awake 호출

            ApplySafeArea();
        }

        /// <summary>
        /// 현재 오브젝트의 RectTransform을 safeArea에 맞게 조정합니다.
        /// </summary>
        protected void ApplySafeArea()
        {
            var canvas = GetComponentInParent<Canvas>();
            var rectTransform = GetComponent<RectTransform>();
            if (canvas == null || rectTransform == null)
                return;

            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            // Canvas의 해상도 기준으로 정규화
            var canvasRect = canvas.pixelRect;
            anchorMin.x /= canvasRect.width;
            anchorMin.y /= canvasRect.height;
            anchorMax.x /= canvasRect.width;
            anchorMax.y /= canvasRect.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        protected override void OnClick(Button button) { }

        public override void Close()
        {
            UIManager.Instance.CloseMenu(name);
        }
    }
}
