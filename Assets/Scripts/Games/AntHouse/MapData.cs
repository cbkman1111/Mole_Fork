using Ant;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MapData
{
    public int mapID = 0;
    public List<TileData> tiles = null;
    

    public static string GetKey(int no)
    {
        return $"map_{no}";
    }

    public MapData(int mapID)
    {
        this.mapID = mapID;
        this.tiles = new List<TileData>();
    }

    public List<TileData> GetTiles()
    {
        return tiles;
    }

    public void Add(TileData tile)
    {
        tiles.Add(tile);
    }

    public void Save()
    {
        PlayerPrefs.SetString(GetKey(mapID), JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }
}
