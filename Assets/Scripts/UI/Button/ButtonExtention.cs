using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Extention
{

    public class ButtonExtention : UnityEngine.UI.Button, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        /// <summary>
        /// Ŭ�� ����.
        /// </summary>
        [SerializeField] public int ClickSound = 0;

        public virtual void Click(PointerEventData eventData = null)
        {
            if (eventData != null)
            {
                base.OnPointerClick(eventData);
            }
        }
    }
}
