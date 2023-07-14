using SweetSugar.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneMatch3 : SceneBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init(JSONObject param)
    {
        /*
        UIMenuMatch3 menu = UIManager.Instance.OpenMenu<UIMenuMatch3>("UIMenuMatch3");
        if (menu != null)
        {
            menu.InitMenu();
            menu.OnStartGame = (int level) => {
                //LevelManager.THIS.gameStatus = GameState.PrepareGame;
                //GUIUtils.THIS.StartGame();
            };
        }
        */

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
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