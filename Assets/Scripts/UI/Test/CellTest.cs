using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellTest : UIObject
{
    public override void OnInit()
    {

    }

    public void SetUI(string text)
    {
        SetText("Text - Number", text);
    }

    public override void Close()
    {
    }

    protected override void OnClick(Button btn)
    {
    }
}
