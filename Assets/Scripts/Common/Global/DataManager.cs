using UnityEngine;
using Common.Global.Singleton;
using Newtonsoft.Json;
using System.Collections.Generic;
using Common.Table;
using System;

namespace Common.Global
{
    public class DataManager : MonoSingleton<DataManager>
    {
        private readonly Dictionary<string, DataTable> tables = new();
        private bool loaded = false;

        protected override bool Init()
        {
            tables.Clear();
            loaded = false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            if (loaded == true)
                return;
   
            TextAsset[] array = ResourcesManager.Instance.LoadAddressableAll<TextAsset>("Table");
            for(int i = 0; i < array.Length; i++)
            {
                var asset = array[i];
                Type tableType = Type.GetType($"Common.Table.{asset.name}");
                if (tableType == null)
                {
                    Debug.LogError($"Class {asset.name} not found");
                    continue;
                }

                // JSON 문자열을 해당 클래스의 인스턴스로 역직렬화합니다.
                object table = JsonConvert.DeserializeObject(asset.ToString(), tableType);
                tables.Add(asset.name, table as DataTable);
            }

            loaded = true;
        }

        
        public T Get<T>()
        {
            string key = typeof(T).Name;

            if(tables.TryGetValue(key, out DataTable value))
            {
                return (T)(object)value;
            }

            return default;
        }
    }
}