using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : UnitBase
{

    public override void SetAngle()
    {
        skel.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
