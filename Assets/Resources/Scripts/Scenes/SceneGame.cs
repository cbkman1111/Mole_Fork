using UnityEngine;

public class SceneGame : SceneBase
{
    public SceneGame(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        UIManager.Instance.OpenPopup<PopupSample>("PopupSample");
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
