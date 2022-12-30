using Ant;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ObjectData
{
    public int id;
    

    public ObjectData()
    {
        id = 0;
        
    }

    public static string GetKey()
    {
        return $"monster";
    }

    public void Save()
    {
        PlayerPrefs.SetString(GetKey(), JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }
}
