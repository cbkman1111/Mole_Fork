using Ant;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PlayerData
{
    public int exp = 0;
    public int level = 0;

    public PlayerData()
    {
        exp = 0;
        level = 0;
    }

    public static string GetKey()
    {
        return $"player";
    }

    public void Save()
    {
        PlayerPrefs.SetString(GetKey(), JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }
}
