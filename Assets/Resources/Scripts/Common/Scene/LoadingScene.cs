using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadingScene : SceneInfo
{
    public LoadingScene(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        CameraManager.Instance.MainCamera.backgroundColor = Color.cyan;
        return true;
    }
}
