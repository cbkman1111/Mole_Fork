using UnityEngine;

public class SceneGame : SceneBase
{
    public SceneGame(SCENES scene) : base(scene)
    {

    }

    public override bool Init(Camera camera)
    {
        mainCamera = camera;

        UIManager.Instance.OpenMenu<UIGameMenu>("UIGameMenu");
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
