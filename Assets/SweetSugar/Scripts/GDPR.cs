using UnityEngine;

namespace SweetSugar.Scripts
{
    public class GDPR : MonoBehaviour
    {
        public GDPRPopupManager go;
    
        void Start()
        {
            Invoke("CheckForGDPR", 1f);
        }
    
        private void CheckForGDPR()
        {
            if (PlayerPrefs.GetInt("npa", -1) == -1)
            {
                go.gdprObject.SetActive(true);
                Time.timeScale = 0;
            }
        }

        public void OnUserClickAccept()
        {
            PlayerPrefs.SetInt("npa", 0);
            go.gdprObject.SetActive(false);
            Time.timeScale = 1;
        }

        public void OnUserClickCancel()
        {
            PlayerPrefs.SetInt("npa", 1);
            go.gdprObject.SetActive(false);
            Time.timeScale = 1;
        }

        public void OnUserClickPrivacyPolicy()
        {
            Application.OpenURL(""); //Enter your privacy policy url
        }
    }
}