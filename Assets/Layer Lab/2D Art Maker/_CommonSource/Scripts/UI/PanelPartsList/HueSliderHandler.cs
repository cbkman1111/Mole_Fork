using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace LayerLab.ArtMaker
{
    /// <summary>
    /// Hue 슬라이더 전용 이벤트 핸들러
    /// Dedicated event handler for Hue slider
    /// </summary>
    public class HueSliderHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Action<float> OnHueChanged;
        
        private RectTransform _rectTransform;
        private bool _isDragging = false;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            UpdateHue(eventData);
            Debug.Log("HueSliderHandler: Pointer Down");
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                UpdateHue(eventData);
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
            Debug.Log("HueSliderHandler: Pointer Up");
        }
        
        private void UpdateHue(PointerEventData eventData)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                var rect = _rectTransform.rect;
                float hue;
                
                if (rect.width > rect.height) // 수평 슬라이더
                {
                    hue = Mathf.Clamp01((localPoint.x - rect.x) / rect.width);
                }
                else // 수직 슬라이더
                {
                    hue = Mathf.Clamp01((localPoint.y - rect.y) / rect.height);
                }
                
                OnHueChanged?.Invoke(hue);
                Debug.Log($"HueSliderHandler: Hue changed to {hue}");
            }
        }
    }
}