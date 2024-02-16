using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOverride : TestBase
{
    public override void foo()
    {
        Debug.Log("TestOverride foo");
    }
}
