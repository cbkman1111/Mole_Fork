using Ant;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ObjectData
{
    public int id;
    public Vector3 position;

    public ObjectData()
    {
        id = 0;
        position = new Vector3(0, 0, 0);
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
