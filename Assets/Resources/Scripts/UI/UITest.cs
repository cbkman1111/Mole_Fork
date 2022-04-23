using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MenuBase
{
    Queue<Block> blocks = new Queue<Block>();

    public bool Init()
    {
        return true;
    }

    public override void Close()
    {
    }

    public override void OnClick(Button btn)
    {
        //throw new System.NotImplementedException();
        string name = btn.name;

        if(CompareTo(name, "Button - Get") == 0)
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.LOADING);
        }
        else if (CompareTo(name, "Button - Release") == 0)
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.GAME);
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
