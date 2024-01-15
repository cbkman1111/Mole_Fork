using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisTile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer = null;
    public Vector2 Coordnate { get; set; } = Vector2.zero;

    public int Value { get; private set; }

    public bool SetValue(int value)
    {
        this.Value = value;

        SetColor();
        return true;
    }

    private void SetColor()
    {
        if (Value == -1)
        {
            spriteRenderer.color = Color.green;
        }
        else if (Value == 0)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.black;
        }
    }
}
