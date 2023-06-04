using System;
using SweetSugar.Scripts.System.Orientation;
using UnityEngine;

namespace SweetSugar.Scripts.GUI
{
    public class Taptoskip : MonoBehaviour
    {
        public Vector2 verticalOrientationPosition;
        public Vector2 horizontalOrientationPosition;
        public RectTransform rectTransform;
        private void Start()
        {
            if (OrientationGameCameraHandle.currentOrientation == ScreenOrientation.Portrait)
                rectTransform.anchoredPosition = verticalOrientationPosition;
            else
                rectTransform.anchoredPosition = horizontalOrientationPosition;
        }
    }
}