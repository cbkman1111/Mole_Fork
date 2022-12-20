using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer spriteRenderer = null;
    public SpriteRenderer spriteRendererBack = null;
    public MeshRenderer meshRenderer = null;
    public SpriteRenderer spriteRendererDebug = null;

    public Rigidbody rigid { get; set; }
    public int Num { get; set; }
    public KindOf KindOfCard { get; set; }
    public int Month { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
    public bool Open { get; set; }
    public bool CompleteMove { get; set; }
    private Action completeMove = null;

    private Board.Player owner = Board.Player.NONE;
    public Board.Player Owner {
        get 
        {
            return owner;
        } 
        set 
        {
            owner = value;
            if(owner == Board.Player.USER)
                spriteRendererDebug.color = Color.blue;
            else if(owner == Board.Player.COMPUTER)
                spriteRendererDebug.color = Color.red;
            else
                spriteRendererDebug.color = Color.white;
        } 
    }

    public enum KindOf { 
        GWANG,
        GWANG_B,

        HONG,
        CHUNG,
        CHO,
        CHO_B,

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
        Month = GetMonth(num);
        Open = false;
        CompleteMove = false;
        Owner = Board.Player.NONE;

        spriteRenderer.sprite = sprite;
        spriteRendererBack.sprite = sprite;

        var collider = GetComponent<BoxCollider>();
        Height = collider.size.y;
        Width = collider.size.x;

        rigid = GetComponent<Rigidbody>();
        gameObject.name = $"card_{num}";

        switch (Num)
        {
            case 1:
            case 9:
            case 29:
            case 41:
                KindOfCard = KindOf.GWANG;
                break;
            case 45:
                KindOfCard = KindOf.GWANG_B;
                break;
            case 5:
            case 13:
            case 30:
                KindOfCard = KindOf.MUNG_GODORI;
                break;
            case 33:
                KindOfCard = KindOf.MUNG_KOO;
                break;
            case 17:
            case 21:
            case 25:
            case 37:
            case 46:
                KindOfCard = KindOf.MUNG;
                break;
            case 2:
            case 6:
            case 10:
                KindOfCard = KindOf.HONG;
                break;
            case 14:
            case 18:
            case 26:
                KindOfCard = KindOf.CHO;
                break;
            case 22:
            case 34:
            case 38:
                KindOfCard = KindOf.CHUNG;
                break;
            case 47:
                KindOfCard = KindOf.CHO_B;
                break;
            case 49:
                KindOfCard = KindOf.PPP;
                break;
            case 50:
            case 51:
            case 52:
                KindOfCard = KindOf.PP;
                break;
            default:
                KindOfCard = KindOf.P;
                break;
        }
        

        SetOpen(false);
        SetPhysicDiable(true);
        return true;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    public int GetMonth(int num)
    {
        return (int)Mathf.Floor((num - 1) / 4 + 1);
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
        Open = false;
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
        Open = false;
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

    public void ShowEnemy(iTween.EaseType ease = iTween.EaseType.linear, float interval = 0.1f, float delay = 0.0f)
    {
        Open = false;
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
    

    public void SetShadow(bool active)
    {
        if (active == true)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        else
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
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
