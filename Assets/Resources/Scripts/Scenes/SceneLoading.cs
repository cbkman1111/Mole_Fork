using UnityEngine;

public class SceneLoading : SceneBase
{
    public SceneLoading(SCENES scene) : base(scene)
    {
    }

    public override bool Init(Camera camera)
    {
        mainCamera = camera;

        AppManager.Instance.ChangeScene(SCENES.GAME);
        return true;
    }
    public override void OnTouchBean(Vector3 position)
    {

    }

    public override void OnTouchEnd(Vector3 position)
    {

    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}
