using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollTest : ScrollBase<ScrollData>
{
    public override bool Init(RectTransform prefab)
    {
        if (base.Init(prefab) == false)
        {
            return false;
        }
        
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

    protected override void UpdateCell(ScrollData data, RectTransform receTrans)
    {
        if (receTrans == null)
            return;

        CellTest cell = receTrans.GetComponent<CellTest>();
        if(cell)
        {
            cell.SetUI(data);
        }
    }

}

public class ScrollData
{
    public string name;
}

