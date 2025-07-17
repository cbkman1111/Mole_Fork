using UnityEngine;

namespace LayerLab.ArtMaker
{
    public class OnlyEditor : MonoBehaviour
    {
        private void Start()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
