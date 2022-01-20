using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : SceneInfo
{
    public GameScene(SCENES scene) : base(scene)
    {

    }

    public override bool Init()
    {
        CameraManager.Instance.MainCamera.backgroundColor = Color.red;

        int w = 100;
        int h = 100;
        Block center = null;

        var pool = Pool.Create();
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Block block = pool.GetBlock();
                if (block != null)
                {
                    Vector2 position = new Vector2();

                    position.x += x * 1;
                    position.y += y * 1;

                    block.transform.SetParent(null);
                    block.transform.position = position;
                    //blocks.Enqueue(block);
                }

                if(x == 50 && y == 50)
                {
                    center = block;
                }
            }
        }

        Vector3 localPosition = center.transform.position;
        CameraManager.Instance.MainCamera.transform.position = new Vector3(localPosition.x, localPosition.y, -1);

        return true;
    }
}
