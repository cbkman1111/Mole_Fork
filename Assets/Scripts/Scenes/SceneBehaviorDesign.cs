using Common.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBehaviorDesign : SceneBase
{
    public Joystick joystick = null;
    [SerializeField]
    public Character player = null;
    [SerializeField]
    public Character enemy = null;


    public override bool Init(JSONObject param)
    {
        joystick.Init((Vector3 direct, float angle) => {
            direct.z = direct.y;

            //playerController.Direction = direct;

            
        },
        () =>
        {
            //playerController.Direction = Vector3.zero;
            
        });

        return true;
    }
}
