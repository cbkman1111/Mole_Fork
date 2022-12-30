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
    public List<ObjectData> monsters = null;

    public static string GetKey(int no)
    {
        return $"map_00_{no}";
    }

    public MapData(int mapID)
    {
        this.mapID = mapID;
        this.tiles = new List<TileData>();
        this.monsters = new List<ObjectData>();
    }

    public List<TileData> GetTiles()
    {
        return tiles;
    }

    public List<ObjectData> GetMonsters()
    {
        return monsters;
    }


    public void AddTile(TileData tile)
    {
        tiles.Add(tile);
    }

    public void AddMonster(ObjectData data)
    {
        monsters.Add(data);
    }

    public void Save()
    {
        PlayerPrefs.SetString(GetKey(mapID), JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }
}
