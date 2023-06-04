using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public static class CameraUtils
    {
        public static Vector2 WorldToScreenPoint2d(this Camera camera, Vector3 worldPoint)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
            return new Vector2(screenPoint.x, screenPoint.y);
        }

        public static Vector3 ScreenToWorldPoint2d(this Camera camera, Vector2 screenPoint)
        {
            return camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, camera.nearClipPlane));
        }
    }
}