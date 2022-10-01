using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer spriteRenderer = null;

    public Rigidbody rigid { get; set; }
    public int Num { get; set; }
    public KindOf Type { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
    public bool Open { get; set; }
    public bool CompleteMove { get; set; }
    private Action completeMove = null;

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
        Open = false;
        CompleteMove = false;

        spriteRenderer.sprite = sprite;
        
        var collider = GetComponent<BoxCollider>();
        Height = collider.size.y;
        Width = collider.size.x;

        rigid = GetComponent<Rigidbody>();
        gameObject.name = $"card_{num}";

        SetOpen(false);
        SetPhysicDiable(true);
        return true;
    }


    public int Kind { get => GetKind(); }
    private int GetKind()
    {
        return (int)Mathf.Floor((Num - 1) / 4 + 1);
    }

    public void SetOpen(bool open)
    {
        Open = open;
        if (Open == true)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 360);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 180); 
        }
    }

    public void SetPhysicDiable(bool active)
    {
        rigid.isKinematic = active;
    }

    public void MoveTo(Vector3 position, iTween.EaseType ease = iTween.EaseType.linear, float time = 0.5f, float delay = 0f, Action complete = null)
    {
        completeMove = complete;
        CompleteMove = false;
        iTween.MoveTo(gameObject, iTween.Hash(
           "x", position.x,
           "y", position.y,
           "z", position.z,
           "delay", delay,
           "time", time,
           "easeType", ease,
           "oncompletetarget", gameObject,
           "oncomplete", "OnMoveComplete"));
    }

    public void CardOpen(iTween.EaseType ease = iTween.EaseType.linear, float interval = 0.1f, float delay = 0.0f)
    {
        iTween.RotateTo(gameObject, iTween.Hash(
                "x", 0,
                "y", 0,
                "z", 360,
                "delay", delay,
                "time", interval,
                "easeType",
                ease,
                "oncompletetarget", gameObject,
                "oncomplete", "OnOpenComplte"));
    }

    public void ShowMe(iTween.EaseType ease = iTween.EaseType.linear, float interval = 0.1f, float delay = 0.0f)
    {
        iTween.RotateTo(gameObject, iTween.Hash(
                "x", -45,
                "y", 0,
                "z", 360,
                "delay", delay,
                "time", interval,
                "easeType",
                ease,
                "oncompletetarget", gameObject,
                "oncomplete", "OnOpenComplte"));
    }

    private void OnOpenComplte()
    {
        Open = true;
    }

    private void OnMoveComplete()
    {
        CompleteMove = true;
        if (completeMove != null)
        {
            completeMove();
            completeMove = null;
        }
    }
}
