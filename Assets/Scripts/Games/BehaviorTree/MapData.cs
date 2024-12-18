using Creature;
using System.Collections.Generic;
using UnityEngine;

namespace Games.BehaviorTree.Datas
{
    [System.Serializable]
    public class Coordinate
    {
        public int X;
        public int Z;

        public List<ObjectData> Objects;
    }
    
    [System.Serializable]
    public class ObjectData
    {
        public int Id;
        [System.NonSerialized] public WorldObject obj;
    }
    
    [System.Serializable]
    public class MapData
    {
        public static int Id = 1000;
       
        public List<Coordinate> Data;

        public MapData()
        {
            Data = new List<Coordinate>();
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
