using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneGostop : SceneBase
{

    private Board board = null;

    public SceneGostop(SCENES scene) : base(scene)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init()
    {
        UIMenuGostop menu = UIManager.Instance.OpenMenu<UIMenuGostop>("UIMenuGostop");
        if (menu != null)
        {
            menu.InitMenu();
        }

        board = Board.Create();
        board.StartGame();

        return true;
    }

    /*

    */

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            
        }
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