using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIObject
{
    Queue<Block> blocks = new Queue<Block>();

    public override void OnClick(Button btn)
    {
        //throw new System.NotImplementedException();
        string name = btn.name;

        if(CompareTo(name, "Button - Get") == 0)
        {
            AppManager.Instance.ChangeScene(SceneInfo.SCENES.LOADING);
            /*
            Block block = GameManager.Instance.GetPool().GetBlock();
            if(block != null)
            {
                block.transform.SetParent(null);
                blocks.Enqueue(block);
            }
            */
        }
        else if (CompareTo(name, "Button - Release") == 0)
        {
            AppManager.Instance.ChangeScene(SceneInfo.SCENES.GAME);
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

        //Debug.Log(btn);
    }
}
