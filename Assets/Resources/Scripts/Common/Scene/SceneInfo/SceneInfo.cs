using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneInfo 
{
    public enum SCENES
    {
        INTRO = 0,
        LOADING,
        GAME,
    }

    //public string name;
    public SCENES scene;
    
    public SceneInfo(SCENES scene)
    {
        //this.name = name;
        this.scene = scene;
    }

    public abstract bool Init();
}
