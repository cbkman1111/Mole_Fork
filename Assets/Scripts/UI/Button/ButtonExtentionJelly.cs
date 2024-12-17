using DG.Tweening;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Extention
{
    public class ButtonExtentionJelly : ButtonExtention
    {
        /// <summary>
        /// Event Button Down
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            const float scale = 1.2f;
            const float duration = 0.1f;
            transform.DOScale(scale, duration);//.SetEase(Ease.InElastic);
        }

        /// <summary>
        /// Button Up.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            Click(eventData);
        }

        public override void Click(PointerEventData eventData = null)
        {
            const float duration = 0.2f;
            transform.DOScale(Vector3.one, duration: duration).
                SetEase(Ease.InOutBack).
                OnComplete(() => {
                    if (eventData != null)
                    { 
                        base.OnPointerClick(eventData);
                    }
                });
        }

        /// <summary>
        /// PointUp에서 Click 처리.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerClick(PointerEventData eventData) { }
    }
}
