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
        public int Width = 1000;
        public int Height = 1000;

        public int X = 500;
        public int Z = 500;
        
        public Dictionary<int, TileData> TileData = new Dictionary<int, TileData>();
        public Dictionary<int, ObjectData> ObjectData = new Dictionary<int, ObjectData>();
        
        public void Save()
        {
            PlayerPrefs.SetString("tile_map", JsonUtility.ToJson(this));
            PlayerPrefs.Save();
        }

        public static MapData Load()
        {
            return JsonUtility.FromJson<MapData>("tile_map");
        }
    }
}
