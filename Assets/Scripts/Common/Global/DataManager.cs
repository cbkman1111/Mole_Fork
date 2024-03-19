using UnityEngine;
using Common.Global.Singleton;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Common.Table;
using System;
using LitJson; // Add this import statement

namespace Common.Global
{
    public class DataManager : MonoSingleton<DataManager>
    {
        private readonly Dictionary<string, DataTable> tables = new();

        protected override bool Init()
        {
            tables.Clear();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadTables()
        {   
            string path = "TableData/";
            string[] tableNames = { 
                "TableTemp1", 
                "TableTemp2",
            }; 

            foreach (string tableName in tableNames)
            {
                TextAsset asset = ResourcesManager.Instance.LoadInBuild<TextAsset>($"{path}{tableName}");
                Type tableType = Type.GetType($"Common.Table.{tableName}");
                if (tableType == null)
                {
                    Debug.LogError($"Class {tableName} not found");
                    continue;
                }

                // JSON 문자열을 해당 클래스의 인스턴스로 역직렬화합니다.
                object table = JsonConvert.DeserializeObject(asset.ToString(), tableType);
                tables.Add(tableName, table as DataTable);
            }
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