using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollTest : ScrollBase<ScrollData>
{
    public bool Init(RectTransform prefab)
    {
        cellPrefab = prefab;
        return true;
    }

    public bool SetItems(List<ScrollData> list)
    {
        if (list == null)
        {
            return false;
        }

        Set(list);
        return true;
    }

    protected override void UpdateCell(RectTransform transform, int index)
    {
        CellTest cell = transform.GetComponent<CellTest>();
        if(cell)
        {
            var info = dataList[index];
            
            cell.SetUI(info.name);
        }
    }
}

public class ScrollData
{
    public int no;
    public int count;
    public string name;
}

