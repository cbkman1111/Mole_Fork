using System;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public Vector3Int Cordinate;

    public TileData(Vector3Int cordinate)
    {
        Cordinate = cordinate;
    }
}
