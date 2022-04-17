using UnityEngine;

public class SceneIntro : SceneBase
{
    public SceneIntro(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        var ui = UIManager.Instance.OpenUI<UITest>("UITest");
        
        CameraManager.Instance.MainCamera.backgroundColor = Color.green;
        return true;
    }
}
