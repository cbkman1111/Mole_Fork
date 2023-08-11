using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MenuBase
{
    public override void OnInit()
    {

    }

    public bool InitMenu()
    {
        return true;
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if(name == "Button - Start1")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.TileMap);
        }
        else if (name == "Button - Start2")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.Gostop);
        }
        else if (name == "Button - Test")
        {
            
        }
        else if (name == "Button - Start4")
        {
            JSONObject jsonParam = new JSONObject();
            jsonParam.SetField("map_no", 3);
            AppManager.Instance.ChangeScene(SceneBase.SCENES.AntHouse, param: jsonParam);
        }
        else if (name.CompareTo("Button - Start5") == 0)
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.Match3);
        }
        else if (name.CompareTo("Button - ChattScroll") == 0)
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.ChattScroll);
        }
    }


}
