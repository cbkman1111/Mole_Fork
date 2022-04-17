using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public const int MAX_COUNT = 10000;

    private Block prefabBlock = null;
    private Queue<Block> poolBlocks = null;

    public static Pool Create()
    {
        GameObject gameObject = new GameObject("Pool");
        if (gameObject != null)
        {
            Pool pool = gameObject.AddComponent<Pool>();
            if (pool.Init() == true)
            {
                return pool;
            }
        }

        return null;
    }

    public bool Init()
    {
        prefabBlock = Resources.Load<Block>("Prefabs/Block");
        if (prefabBlock == null)
        {
            Debug.LogError("prefabBlock can not be null.");
        }

        poolBlocks = new Queue<Block>();
        for (int i = 0; i < MAX_COUNT; i++)
        {
            Block block = Instantiate<Block>(prefabBlock, transform);
            if (block != null)
            {
                block.name = block.name.Replace("(Clone)", "");
                block.gameObject.SetActive(false);
                poolBlocks.Enqueue(block);
            }
        }

        return true;
    }

    public Block GetBlock()
    {
        if(poolBlocks.Count == 0)
        {
            Debug.Log("no more insatnace.");
            return null;
        }

        Block block = poolBlocks.Dequeue();
        block.gameObject.SetActive(true);
        return block;
    }

    public bool ReleaseBlock(Block block)
    {
        if (poolBlocks.Contains(block) == true)
        {
            return false;
        }

        block.transform.SetParent(transform);
        block.gameObject.SetActive(false);
        poolBlocks.Enqueue(block);
        return true;
    }
}
