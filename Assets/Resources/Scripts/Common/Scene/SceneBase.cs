using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase
{
    public enum SCENES
    {
        INTRO = 0,
        LOADING,
        GAME,
    }

    public SCENES scene;
    
    public SceneBase(SCENES scene)
    {
        this.scene = scene;
    }

    public abstract bool Init();
    public abstract void OnTouchBean(Vector3 position);
    public abstract void OnTouchMove(Vector3 position);
    public abstract void OnTouchEnd(Vector3 position);
}
