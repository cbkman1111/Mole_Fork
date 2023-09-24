using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ant
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField]
        public RectTransform RectTrans = null;
        [SerializeField]
        public RectTransform Handler = null;
        
        private bool clicked = false;
        private float radius = 0;
        private Vector3 center = Vector3.zero;
        
        public Action<Vector3, float> OnMove { get; set; }
        public Action OnStop { get; set; }

        public bool Init(Action<Vector3, float> move, Action stop)
        {
            center = RectTrans.position;
            radius = RectTrans.rect.width * 0.5f;
            OnMove = move;
            OnStop = stop;

            return true;
        }

        float GetAngle(Vector2 start, Vector2 end)
        {
            Vector2 v2 = end - start;
            return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        }

        public void TouchBegin(Vector3 position)
        {
            float distance = Mathf.Abs(Vector3.Distance(center, position));
            if (distance <= radius)
            {
                clicked = true;
            }
        }

        public void TouchMove(Vector3 position)
        {
            if (clicked == true)
            {
                Handler.position = position;

                var distance = Mathf.Abs(Vector2.Distance(center, position));
                var angle = GetAngle(center, position);
                var newDirection = (position - center).normalized;

                if (distance > radius)
                {
                    var edge = newDirection * radius;
                    Handler.localPosition = edge;

                    Debug.DrawRay(center, edge, Color.blue);
                }

                OnMove(newDirection, angle);
            }
        }

        public void TouchEnd(Vector3 position)
        {
            if (clicked == true)
            {
                OnStop();
            }

            Handler.localPosition = Vector3.zero;
            clicked = false;
        }
    }
}
