using SweetSugar.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneMatch3Buyed : SceneBase
{
    public SceneMatch3Buyed(SCENES scene) : base(scene)
    {
    }

    public override bool Init(JSONObject param)
    {
        UIMenuMatch3Buyed menu = UIManager.Instance.OpenMenu<UIMenuMatch3Buyed>("UIMenuMatch3Buyed");
        if (menu != null)
        {
            menu.InitMenu();
        }

        return true;
    }

    private void Update()
    {

    }

    public override void OnTouchBean(Vector3 position)
    {

    }

    public override void OnTouchEnd(Vector3 position)
    {
       
    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}