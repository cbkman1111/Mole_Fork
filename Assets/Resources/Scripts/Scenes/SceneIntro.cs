using UnityEngine;

public class SceneIntro : SceneBase
{
    public SceneIntro(SCENES scene) : base(scene)
    {
    }

    public override bool Init()
    {
        var ui = UIManager.Instance.OpenMenu<UITest>("UITest");
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

