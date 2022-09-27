using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMenu : SceneBase
{
    public SceneMenu(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        UIMenu menu = UIManager.Instance.OpenMenu<UIMenu>("UIMenu");
        if (menu != null)
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
