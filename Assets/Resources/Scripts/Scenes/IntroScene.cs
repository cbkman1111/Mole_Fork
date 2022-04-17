using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IntroScene : SceneBase
{
    public IntroScene(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        UIManager.Instance.OpenUI<UITest>("UITest");

        CameraManager.Instance.MainCamera.backgroundColor = Color.green;
        return true;
    }
}

