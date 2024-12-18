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
        //public Ease tweenType = Ease.Linear;
        //public float speed = 0;
        //public float inverval = 0f;
        //public float scaleTime = 0f;

        private UIMenuTest menu = null;
        //private int width = 0;
        //private int height = 0;
        //private float size = 1.0f;

        //public PoolManager poolManager = null;

        //public MeshRenderer meshRender = null;

        public Pool<SpriteRenderer> poolSprite = null;
        public Transform targetCube = null;
        public Sprite[] icons;

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
                menu.ClickCreateObject = CreateObject;
            }

            var prefab = ResourcesManager.Instance.LoadInBuild<SpriteRenderer>("Star");
            poolSprite = Pool<SpriteRenderer>.Create(prefab, targetCube, 30);
            return true;
        }

        /// <summary>
        /// 객체 생성.
        /// </summary>
        private void CreateObject()
        {
            float particleCount = UnityEngine.Random.Range(10f, 30f);
            int index = (int)UnityEngine.Random.Range(0f, icons.Length);
            Sprite icon = icons[index];

            for (int i = 0; i < (int)particleCount; i++)
            {
                var sprite = poolSprite.GetObject();
                sprite.transform.localPosition = Vector3.zero;
                sprite.sortingOrder = i;
                sprite.sprite = icon;
                sprite.color = new Color(1, 1, 1, 1);

                // 360도 랜덤 각도 생성
                float angle = UnityEngine.Random.Range(0f, 360);
                // 각도를 라디안으로 변환
                float radians = angle * Mathf.Deg2Rad;
                // 점프할 거리 설정 (예: 5 유닛)
                float distance = UnityEngine.Random.Range(2f, 5f);

                // 점프할 위치 계산
                Vector3 jumpTarget = new Vector3(
                    Mathf.Cos(radians) * distance,
                    0,
                    Mathf.Sin(radians) * distance
                );

                float rand = UnityEngine.Random.Range(200f, 500f);
                float power = rand / 100f;
                int numJumps = 1;
                float duration = Mathf.Lerp(0.4f, 0.6f, (power - 1f) / 4f);
                
                var sequnce = DOTween.Sequence();
                sequnce.Append(sprite.transform.DOJump(
                     jumpTarget,
                     power,
                     numJumps,
                     duration).
                     SetEase(Ease.Linear));
                sequnce.AppendInterval(0.2f);
                sequnce.Append(sprite.DOFade(0, 0.5f));
                sequnce.AppendCallback(() =>
                {
                    poolSprite.ReturnObject(sprite);
                });

                sequnce.Play();
            }
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