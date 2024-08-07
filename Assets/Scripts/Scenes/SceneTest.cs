using System;
using System.Collections.Generic;
using Common.Global;
using Common.Scene;
using Common.Utils.Pool;
using DG.Tweening;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneTest : SceneBase
    {


        public Ease tweenType = Ease.Linear;
        public float speed = 0;
        public float inverval = 0f;
        public float scaleTime = 0f;

        private UIMenuTest menu = null;
        //private int width = 0;
        //private int height = 0;
        //private float size = 1.0f;

        public PoolManager poolManager = null;

        public MeshRenderer meshRender = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UIMenuTest>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            return true;
        }

        /// <summary>
        /// 미리 로딩해야 할 데이터 처리.
        /// </summary>
        public async override void Load(Action<float> update)
        {
            int total = 99000000;
            List<int> list = new List<int>();
            for (int i = 0; i < total; i++)
            {
                float percent = (float)i / (float)total;
                list.Add(i);

                update(percent);
            }

            update(1f);
        }

        public override void OnTouchBean(Vector3 position)
        {
            Ray ray = MainCamera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Tile"))
                {
                    var obj = hit.collider.gameObject;
               
                }

                Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
                Debug.Log(hit.point);
            }
        }

        public override void OnTouchEnd(Vector3 position)
        {

        }

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {

        }
    }
}