using Common.Global;
using Common.Scene;
using Common.UIObject;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuHash : MenuBase
    {
        // Start is called before the first frame update
        [SerializeField]
        private TMP_Text textResult;
        [SerializeField]
        private TMP_InputField InputField;
        
        private Action<string> OnConvertSHA;
        private Action<string> OnConvertMD5;
        private Action<string> OnConvertBase64;

        public bool InitMenu(Action<string> convertSHA256, Action<string> convertMD5, Action<string> convertBase64)
        {
            OnConvertSHA = convertSHA256;
            OnConvertMD5 = convertMD5;
            OnConvertBase64 = convertBase64;
            return true;
        }

        public void SetResult(string msg)
        {
            textResult.text = msg;
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;

            if (name == "Button - Convert SHA")
            {
                OnConvertSHA(InputField.text);
            }
            else if(name == "Button - Convert MD5")
            {
                OnConvertMD5(InputField.text);
            }
            else if(name == "Button - Convert Base64")
            {
                OnConvertBase64(InputField.text);
            }
            else if(name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }
    }
}