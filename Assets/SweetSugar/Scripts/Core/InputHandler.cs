using System;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.GUI.Boost;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SweetSugar.Scripts.Core
{
    public class InputHandler : Singleton<InputHandler>
    {
        private Vector2 mousePos;
        private Vector2 _delta;
        private bool down;
        private Camera _camera;

        public delegate void MouseEvents(Vector2 pos);

        public static event MouseEvents OnDown, OnMove, OnUp, OnDownRight;

        private void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseDown(GetMouseWorldPos());
                down = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                MouseUp(GetMouseWorldPos());
                down = false;
            }
            if (Input.GetMouseButtonDown(1))
                MouseDownRight(GetMouseWorldPos());
            if (Input.GetMouseButton(0) && down)
            {
                MouseMove(GetMouseWorldPos());
            }
        }

        private Vector3 GetMouseWorldPos()
        {
            return _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public void MouseDown(Vector2 pos)
        {
            mousePos = pos;
            OnDown?.Invoke(mousePos);
        }
        
        public void MouseUp(Vector2 pos)
        {
            mousePos = pos;
            OnUp?.Invoke(mousePos);
        }

        public void MouseMove(Vector2 pos)
        {
            _delta = mousePos - pos;
            mousePos = pos;
            OnMove?.Invoke(mousePos);
        }
        
        public void MouseDownRight(Vector2 pos)
        {
            mousePos = pos;
            OnDownRight?.Invoke(mousePos);
        }

        public Vector2 GetMousePosition() => mousePos;
        public Vector2 GetMouseDelta() => _delta;
    }
}