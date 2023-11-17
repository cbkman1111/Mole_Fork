using Common.Global;
using Common.Scene;
using System.Collections.Generic;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneIntro : SceneBase
    {
        public UIMenuIntro menu = null;
        public Animation idle = null;
        public AudioClip[] clips = null;

        public override bool Init(JSONObject param)
        {
            if(menu != null)
            {
                menu.InitMenu();
            }

            idle.Play();

            SoundManager.Instance.InitList(transform, clips);
            //SoundManager.Instance.PlayMusic("17856_1462216818");
            return true;
        }

        public override void OnTouchBean(Vector3 position)
        {

        }

        public override void OnTouchEnd(Vector3 position)
        {

        }

        public override void OnTouchMove(Vector3 position)
        {

        }
    }
}

