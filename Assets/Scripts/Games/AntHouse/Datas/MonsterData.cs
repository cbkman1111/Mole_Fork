using Ant;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MonsterData
{
    public int id;
    public Vector3 position;
    public float speed = 0.5f;

    public MonsterData()
    {
        id = 0;
        position = Vector3.zero;
        speed = 0.01f;
    }
}
