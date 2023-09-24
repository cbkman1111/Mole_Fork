using UnityEngine;

namespace Scenes
{
    public class SceneLoading : MonoBehaviour
    {
        public UILoadingMenu menu = null;

        public void SetPercent(float percent)
        {
            if (menu != null)
            {
                menu.SetPercent(percent);
            }
        }

        public bool Complete()
        {
            return menu.Complete();
        }
    }
}
