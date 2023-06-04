using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    public enum SCENES
    {
        Intro = 0,
        Menu,
        Loading,

        Gostop,
        TileMap,
        AntHouse,
        Match3,

        Test,
    }

    public SCENES Scene { get; set; }
    public Camera MainCamera { get; set; }
    public SceneBase(SCENES scene)
    {
        Scene = scene;
    }

    public virtual void UnLoaded() { }
    public abstract bool Init(JSONObject param);
    public abstract void OnTouchBean(Vector3 position);
    public abstract void OnTouchMove(Vector3 position);
    public abstract void OnTouchEnd(Vector3 position);
}
