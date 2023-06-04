using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SweetSugar.Scripts.System
{
    public class ServerTime : UnityEngine.MonoBehaviour
    {
        public static ServerTime THIS;
        public DateTime serverTime;
        public bool dateReceived;
        public delegate void DateReceived();
        public static event DateReceived OnDateReceived;
        [Header("Test date example: 2019-08-27 09:12:29")]
        public string TestDate; 
        private void Awake()
        {
            if (THIS == null)
                THIS = this;
            else if(THIS != this)
                Destroy(gameObject);
            GetServerTime();
        }

        private void OnEnable()
        {
            GetServerTime();
        }

        void GetServerTime ()
        {
                StartCoroutine(getTime());
        }
  
        IEnumerator getTime()
        {
#if UNITY_WEBGL
            serverTime = DateTime.Now;
#else
            UnityWebRequest www = UnityWebRequest.Get("https://candy-smith.info/gettime.php");
            yield return www.SendWebRequest();
            if(www.downloadHandler.text != "")
                serverTime = DateTime.Parse(www.downloadHandler.text);
            else
                serverTime = DateTime.Now;
            if(TestDate!="" && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor))
                serverTime = DateTime.Parse(TestDate);
#endif
            //Debug.Log(serverTime);
            yield return  null;
            dateReceived = true;
            OnDateReceived?.Invoke();
        }
    }
}