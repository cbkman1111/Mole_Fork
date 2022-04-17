using UnityEngine;

public class SceneLoading : SceneBase
{
    public SceneLoading(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        CameraManager.Instance.MainCamera.backgroundColor = Color.cyan;
        return true;
    }
}
