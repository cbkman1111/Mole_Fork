using UnityEngine;
using UnityEngine.SceneManagement;

namespace SweetSugar.Scripts
{
    public class GDPRPopupManager : MonoBehaviour
    {
        public GameObject gdprObject;
    
        private void Awake()
        {
            if (SceneManager.GetActiveScene().name == "main" && PlayerPrefs.GetInt("npa", -1) == -1)
            { 
                gdprObject = Instantiate(Resources.Load("GDPR") as GameObject, this.transform, false);
                gdprObject.GetComponent<GDPR>().go = this;
            }
        }
    }
}
