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
        position = Vector3.zero;
    }
}
