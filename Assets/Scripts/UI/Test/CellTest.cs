using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellTest : UIObject
{
    public Image color = null;

    public override void OnInit()
    {

    }

    public void SetUI(ScrollData data)
    {
        SetText("Text - Number", data.name);

        float r = UnityEngine.Random.Range(0, 1f);
        float g = UnityEngine.Random.Range(0, 1f);
        float b = UnityEngine.Random.Range(0, 1f);

        GetComponent<Image>().color = new Color(r, g, b);
    }

    public void SetColor(Color c)
    {
        color.color = c;
    }

    public override void Close()
    {
    }

    protected override void OnClick(Button btn)
    {
    }
}
