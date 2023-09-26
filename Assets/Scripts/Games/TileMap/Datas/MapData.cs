using System.Collections.Generic;
using TileMap;
using UnityEngine;

namespace Games.TileMap.Datas
{
    [System.Serializable]
    public class Coordinate
    {
        public int X;
        public int Z;

        public TileData Tile;
        public List<ObjectData> Objects;
    }

    public enum TileType
    {
        Water, // 물.
        Ground, // 일반 땅.
        Wall, // 벽.
    }

    [System.Serializable]
    public class TileData
    {
        public TileType type;

        [System.NonSerialized] public Tile obj;
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
        
        public int Width = 200;
        public int Height = 200;

        public int X = 0;
        public int Z = 0;

        public List<Coordinate> Data;

        public MapData()
        {
            Data = new List<Coordinate>();
            
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
