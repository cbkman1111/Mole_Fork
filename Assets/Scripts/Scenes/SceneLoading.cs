using Common.Global;
using Common.Scene;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scenes
{
    public class SceneLoading : SceneBase
    {
        public UILoadingMenu menu = null;
        private float percent = 0f;

        public override bool Init(JSONObject param)
        {
            MEC.Timing.RunCoroutine(LoadData());
            return true;
        }

        private IEnumerator<float> LoadData()
        {
            while(percent < 1.0f)
            {
                percent += 0.01f;
                menu.SetPercent(percent);
                yield return MEC.Timing.WaitForOneFrame;
            }

            menu.SetPercent(1.0f);
            yield return MEC.Timing.WaitForSeconds(0.5f);
        }
    }
}
