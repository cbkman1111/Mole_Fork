using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X { get; set; }
    public int Y { get; set; }
    public GameObject AttachedObject { get; set; }

    public bool Init()
    {
        AttachedObject = null;
        return true;
    }
}
