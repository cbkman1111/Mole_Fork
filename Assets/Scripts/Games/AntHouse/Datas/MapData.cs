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
    public List<MonsterData> monsters = null;
    public List<ObjectData> objs = null;
    public MonsterData player = null;

    public static string GetKey(int no)
    {
        return $"map_0001_{no}";
    }

    public MapData(int mapID)
    {
        this.mapID = mapID;
        this.tiles = new List<TileData>();
        this.monsters = new List<MonsterData>();
        this.objs = new List<ObjectData>();
        this.player = new MonsterData();
    }

    public List<TileData> GetTiles()
    {
        return tiles;
    }

    public List<MonsterData> GetMonsters()
    {
        return monsters;
    }

    public List<ObjectData> GetObjs()
    {
        return objs;
    }


    public void AddTile(TileData tile)
    {
        tiles.Add(tile);
    }

    public void AddMonster(MonsterData data)
    {
        monsters.Add(data);
    }

    public void AddObject(ObjectData data)
    {
        objs.Add(data);
    }

    public void AddPlayer(MonsterData data)
    {
        player = data;
    }

    public void Save()
    {
        PlayerPrefs.SetString(GetKey(mapID), JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }
}
