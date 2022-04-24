using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    public enum SCENES
    {
        INTRO = 0,
        LOADING,
        GAME,
    }

    public SCENES Scene { get; set; }
    public Camera MainCamera { get; set; }
    public SceneBase(SCENES scene)
    {
        Scene = scene;
    }

    public abstract bool Init();
    public abstract void OnTouchBean(Vector3 position);
    public abstract void OnTouchMove(Vector3 position);
    public abstract void OnTouchEnd(Vector3 position);
}
