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
        board = Board.Create();
        UIMenuGostop menu = UIManager.Instance.OpenMenu<UIMenuGostop>("UIMenuGostop");
        if (menu != null)
        {
            menu.InitMenu(board);
            
        }

        //
        board.OnGameStart = () => {
            menu.ShowScoreMenu(true);
        };
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
        Ray ray = MainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            var layer = hit.collider.gameObject.layer;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Card"))
            {
                Card card = hit.collider.GetComponent<Card>();
                if (card != null)
                {
                    if (board.GetState() == Board.State.CARD_HIT &&
                        board.MyTurn() == true)
                    {
                        board.HitCard(0, card);
                    }
                }
            }
        }
    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}