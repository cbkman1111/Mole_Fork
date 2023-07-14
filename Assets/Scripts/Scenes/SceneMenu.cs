using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMenu : SceneBase
{


    public override bool Init(JSONObject param)
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
        var world = MainCamera.ScreenToWorldPoint(position);

        Ray ray = MainCamera.ScreenPointToRay(world);
        RaycastHit2D hit = Physics2D.Raycast(world, transform.forward);
        if (hit.collider != null)
        {

        }

        Debug.DrawRay(world, transform.forward * 100, Color.red, 1.3f);
    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}
