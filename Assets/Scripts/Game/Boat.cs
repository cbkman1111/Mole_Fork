using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public float angle = 0f;
    public float speed = 0f;
    public Vector2 coord = Vector2.zero;
    public List<Block> list = null;

    public void SetAngle(float angle)
    {
        this.angle = angle;
    }

    public float GetAngle()
    {
        return this.angle;
    }

    public void GenerateNavmesh()
    {
        NavMeshBuilder builder = gameObject.GetComponent<NavMeshBuilder>();
        if (builder != null)
        {
            builder.GenerateNavmesh();
        }
    }
}
