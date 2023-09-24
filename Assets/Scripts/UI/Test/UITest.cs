using System.Collections;
using System.Collections.Generic;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using Games.Sea;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MenuBase
{
    Queue<Block> blocks = new Queue<Block>();

    public bool InitMenu()
    {
        return true;
    }

    public override void Close()
    {
    }

    protected override void OnClick(Button btn)
    {
        //throw new System.NotImplementedException();
        string name = btn.name;

        if(name == "Button - Get")
        {
            AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneLoading);
        }
        else if (name == "Button - Release")
        {
            
            /*
            if(blocks.Count > 0)
            {
                Block block = blocks.Dequeue();
                if (block != null)
                {
                    GameManager.Instance.GetPool().ReleaseBlock(block);
                }
            }
            */
        }
    }
}
