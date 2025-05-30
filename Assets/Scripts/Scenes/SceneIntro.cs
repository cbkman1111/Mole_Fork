using Common.Global;
using Common.Scene;
using Common.Utils;
using DG.Tweening;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneIntro : SceneBase
    {
        [SerializeField] GameObject Cube = null;
        [SerializeField] GameObject CubeCamera = null;
        public float RotationSpeed = 100f;

        public override bool Init(JSONObject param)
        {
            GiantDebug.Log($"SceneIntro init. start.");

            UIMenuIntro menu = UIManager.Instance.OpenMenu<UIMenuIntro>();
            GiantDebug.Log($"SceneIntro init. {menu}");

            if(menu != null)
            {
                menu.InitMenu();
            }
            
            var sequence = DOTween.Sequence();
            var moveUp = MainCamera.transform.DOLocalMove(new Vector3(-0.07f, 2, -4f), 3f).SetEase(Ease.InOutQuad);

            sequence.Append(moveUp)
                    //.Append(moveDown)
                    .SetLoops(-1, LoopType.Yoyo);
            
            sequence.Play();

            //MainCamera.transform.DOLocalMove(new Vector3(0, 3, 0), 1f).SetEase(Ease.InOutQuad).Loops();
            return true;
        }

        public override void OnUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float rotate = 0.25f;
            Cube.transform.Rotate(1,1,1);
            CubeCamera.transform.Rotate(0, rotate, 0);
        }

        public override void OnTouchBean(Vector3 position)
        {

        }

        public override void OnTouchEnd(Vector3 position)
        {

        }

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {

        }
    }
}

