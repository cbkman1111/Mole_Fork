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
}
