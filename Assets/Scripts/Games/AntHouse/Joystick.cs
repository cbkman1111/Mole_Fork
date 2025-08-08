using System;
using UnityEngine;

namespace Ant
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField] private RectTransform RectTrans = null;
        [SerializeField] private RectTransform Handler = null;
        
        private bool clicked = false;
        private float radius = 0;
        private Vector3 center = Vector3.zero;

        public event System.Action<Vector3, float> OnMove;
        public event System.Action OnStop;

        public bool Init()
        {
            radius = RectTrans.rect.width * 0.5f;
            OnMove = null;
            OnStop = null;

            RectTrans.gameObject.SetActive(false);
            return true;
        }

        float GetAngle(Vector2 start, Vector2 end)
        {
            Vector2 v2 = end - start;
            return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        }

        public void TouchBegin(Vector3 position)
        {
            center = position;
            clicked = true;
            RectTrans.position = position;

            RectTrans.gameObject.SetActive(true);
        }

        public void TouchMove(Vector3 position)
        {
            if (clicked == true)
            {
                var distance = Mathf.Abs(Vector2.Distance(center, position));
                var angle = GetAngle(center, position);
                var newDirection = (position - center).normalized;

                if (distance >= radius)
                    Handler.position = center + newDirection * radius;
                else
                    Handler.position = position;

                if(OnMove != null)
                    OnMove(newDirection, angle);
            }
        }

        public void TouchEnd(Vector3 position)
        {
            if (clicked == true)
            { 
                if(OnStop != null)
                    OnStop();
            }

            RectTrans.gameObject.SetActive(false);
            Handler.localPosition = Vector3.zero;
            clicked = false;
        }
    }
}
