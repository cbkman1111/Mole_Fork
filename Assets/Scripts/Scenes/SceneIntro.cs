using UnityEngine;

public class SceneIntro : SceneBase
{
    public SceneIntro(SCENES scene) : base(scene)
    {
    }

    public override bool Init(JSONObject param)
    {
        UIMenuIntro menu = UIManager.Instance.OpenMenu<UIMenuIntro>("UI/UIMenuIntro");
        if(menu != null)
        {
            menu.InitMenu();
        }

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

