using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int Num { get; set; }
    public KindOf Type { get; set; }
    public float Height { get; set; }
    private Rigidbody rigid = null;

    public enum KindOf { 
        GWANG,
        GWANG_B,

        HONG,
        CHUNG,
        CHO,

        MUNG,
        MUNG_GODORI,
        MUNG_KOO,

        P,
        PP,
        PPP,
    }

    public bool Init(int num, Sprite sprite)
    {
        Num = num;

        var front = transform.Find("Front");
        var renderer = front.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        
        var collider = GetComponent<BoxCollider>();
        Height = collider.size.z;

        rigid = GetComponent<Rigidbody>();
        

        gameObject.name = $"card_{num}";

        Flip(false);
        SetKinematic(true);
        return true;
    }


    public int Kind { get => GetKind(); }
    private int GetKind()
    {
        return (int)Mathf.Floor((Num - 1) / 4 + 1);
    }

    public void Flip(bool active)
    {
        if (active == true)
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(-90, 0, 0);
        }   
    }

    public void SetKinematic(bool active)
    {
        rigid.isKinematic = active;
    }
}
