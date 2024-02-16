using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBase : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void foo()
    {
        Debug.Log("TestBase foo");
    }

}
