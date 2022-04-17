using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : SceneBase
{
    public GameScene(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        CameraManager.Instance.MainCamera.backgroundColor = Color.red;
        CameraManager.Instance.MainCamera.transform.position = new Vector3(0f, 0f, -1f);

        
        return true;
    }
}
