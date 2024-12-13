using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Extention
{

    public class ButtonExtention : UnityEngine.UI.Button, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        /// <summary>
        /// 클릭 사운드.
        /// </summary>
        [SerializeField] public int ClickSound = 0;
    }
}
