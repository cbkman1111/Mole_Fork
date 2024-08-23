using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.Utils.Pool;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Spine.Unity.Editor.SpineEditorUtilities;

namespace UI.Menu
{
    public class UIMenuTest : MenuBase
    {
        /*
            public GameObject from;
            public GameObject to;
            public GameObject result;
            public Text angle;

            public int x;
            public int y;
        */

        public Action ClickCreateObject = null;

        public Pool<UIJumpCoin> pool = null;
        public Transform targetCube = null;

        public bool createField = false;

        public bool InitMenu()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<UIJumpCoin>("UIJumpCoin");
            pool = Common.Utils.Pool.Pool<UIJumpCoin>.Create(prefab, targetCube, 30);

            return true;
        }

        /*
        private void Update()
        {
            float radius = 200;
            float a = CalculateAngle(from.transform, to.transform.position, from.transform.position);

            float x = (Mathf.Sin(a * (Mathf.PI / 180)) * radius);
            float y = (Mathf.Cos(a * (Mathf.PI / 180)) * radius);

            Vector3 dest = from.transform.position;
            dest.x += x;
            dest.y += y;
            result.transform.position = dest;

            SetText(angle, $"{a}");
        }
        */

        public static float CalculateAngle(Transform trans, Vector3 from, Vector3 to)
        {
            //Vector3 v = to - from;
            //float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;    // return : -180 ~ 180 degree (for unity)
            float angle = Vector3.SignedAngle(-trans.up, to - from, trans.forward);
            //float angle = Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
            //float angle = Quaternion.Angle(transform.rotation, target.rotation);
            //float angle = Vector3.Angle(to, from);


            return angle;
        }

        public override void OnValueChanged(InputField input, string str) 
        {
            /*
            if (input.name.CompareTo("InputField - x") == 0)
            {
                x = int.Parse(str);
            }
            else if (input.name.CompareTo("InputField - y") == 0)
            {
                y = int.Parse(str);
            }*/
        }

        /// <summary>
        /// 객체 생성.
        /// </summary>
        private void CreateObject()
        {
            float particleCount = UnityEngine.Random.Range(10f, 30f);
            //int index = (int)UnityEngine.Random.Range(0f, icons.Length);
            //Sprite icon = icons[index];

            for (int i = 0; i < (int)particleCount; i++)
            {
                var sprite = pool.GetObject();
                sprite.Index = i;
                sprite.ShowText(false);
                sprite.transform.localPosition = Vector3.zero;
                sprite.canvasGroup.alpha = 1;

                // 360도 랜덤 각도 생성
                float angle = UnityEngine.Random.Range(180f, 360);
                if (i == 0)
                    angle = 270;

                // 각도를 라디안으로 변환
                float radians = angle * Mathf.Deg2Rad;
                // 점프할 거리 설정 (예: 5 유닛)
                float distance = UnityEngine.Random.Range(100f, 200f);
                // 점프할 위치 계산
                Vector3 jumpTarget = new Vector3(
                    Mathf.Cos(radians) * distance,
                    Mathf.Sin(radians) * distance,
                    0
                ) + targetCube.transform.position;

                float power = UnityEngine.Random.Range(100f, 180f);// / 100f;
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
                sequnce.AppendCallback(() => {
                    if (sprite.Index == 0)
                    {
                        var obj = pool.GetObject();
                        obj.canvasGroup.alpha = 1;
                        obj.ShowText(true);
                        obj.transform.SetAsLastSibling();
                        obj.transform.position = sprite.transform.position;
                        float destY = sprite.transform.position.y + 100;
                        var sequnce = DOTween.Sequence();
                        sequnce.Append(obj.transform.DOMoveY(destY, 0.5f));
                        sequnce.AppendInterval(0.2f);
                        sequnce.Append(obj.canvasGroup.DOFade(0, 0.5f));
                        sequnce.AppendCallback(() =>
                        {
                            pool.ReturnObject(obj);
                        });
                    }
                });
                sequnce.Append(sprite.canvasGroup.DOFade(0, 0.5f));
                sequnce.AppendCallback(() =>
                {
                    pool.ReturnObject(sprite);
  
                });

                sequnce.Play();
            }
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (name == "Button - CreateObject")
            {
                if(createField== true)
                    ClickCreateObject?.Invoke();
                else
                    CreateObject();
            }
        }
    }
}
