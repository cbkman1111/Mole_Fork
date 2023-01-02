using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    public enum SCENES
    {
        INTRO = 0,
        MENU,
        LOADING,
        GAME,
        GAME_GOSTOP,
        GAME_TILEMAP,
        GAME_ANTHOUSE,
    }

    public SCENES Scene { get; set; }
    public Camera MainCamera { get; set; }
    public SceneBase(SCENES scene)
    {
        Scene = scene;
    }

    public virtual void UnLoaded() { }
    public abstract bool Init();
    public abstract void OnTouchBean(Vector3 position);
    public abstract void OnTouchMove(Vector3 position);
    public abstract void OnTouchEnd(Vector3 position);
}
