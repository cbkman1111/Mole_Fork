using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.TileMap.Datas
{
    [System.Serializable]
    public class TileData
    {
        public Color Color;
        public int Child;
    }
    
    [System.Serializable]
    public class ObjectData
    {
        public int Id;
    }
    
    [System.Serializable]
    public class MapData
    {
        public static int Id = 102;
        
        public int Width = 500;
        public int Height = 500;

        public int X = 0;
        public int Z = 0;

        public List<TileData> TileData;
        //public Dictionary<int, ObjectData> ObjectData = new Dictionary<int, ObjectData>();

        public MapData()
        {
            TileData = new List<TileData>();
            X = (int)(Width * 0.5f);
            Z = (int)(Height * 0.5f);
        }

        public static string GetKey(int no)
        {
            return $"cbkmap_000{no}";
        }
        
        public void Save()
        {
            var key = GetKey(MapData.Id);
            var json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public static MapData Load()
        {
            string key = GetKey(MapData.Id);
            var str = PlayerPrefs.GetString(key);
            if (str == string.Empty)
                return null;
            
            return JsonUtility.FromJson<MapData>(str);
        }
    }
}
