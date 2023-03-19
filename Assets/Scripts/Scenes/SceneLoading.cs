using System.Collections;
using UnityEngine;

public class SceneLoading : SceneBase
{
    private UILoadingMenu menu = null;

    public SceneLoading(SCENES scene) : base(scene)
    {
    }

    public override bool Init(JSONObject param)
    {
        menu = UIManager.Instance.OpenMenu<UILoadingMenu>("UILoadingMenu");
       

        return true;
    }

    public void SetPercent(float percent)
    {
        if (menu != null)
        {
            menu.SetPercent(percent);
        }
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
