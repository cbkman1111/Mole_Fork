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

    //public string name;
    public SCENES scene;
    
    public SceneBase(SCENES scene)
    {
        //this.name = name;
        this.scene = scene;
    }

    public abstract bool Init();
}
