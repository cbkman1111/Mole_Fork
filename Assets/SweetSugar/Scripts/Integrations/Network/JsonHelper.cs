using System;
using UnityEngine;

namespace SweetSugar.Scripts.Integrations.Network
{
    [Serializable]
    public class JsonHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = null;
            try
            {
                wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
            }
            catch
            {
                Debug.Log("Unexpected nod: " + json);
            }
            return wrapper?.array;
        }
        
        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}