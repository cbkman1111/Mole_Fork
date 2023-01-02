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
        UIMenuGostop menu = UIManager.Instance.OpenMenu<UIMenuGostop>("UI/UIMenuGostop");
        if (menu != null)
        {
            menu.InitMenu();
        }

        board = Board.Create(menu);
        if (board != null)
        {
            board.StartGame();
        }

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
                    var stateMachine = board.GetStateMachine();
                    var turnInfo = stateMachine.GetCurrturnInfo();
                    var stateInfo = turnInfo.GetCurrentStateInfo();

                    if (stateInfo.state == State.CARD_HIT &&
                        board.MyTurn() == true)
                    {
                        var list = board.GetSameMonthCard((int)Board.Player.USER, card);
                        if (list .Count >= 3)
                        {
                            board.HitBomb((int)Board.Player.USER, list);
        

                            stateInfo.evt = StateEvent.PROGRESS; // 카드 침.
                        }
                        else 
                        {
                            board.HitCard((int)Board.Player.USER, card);
                            stateInfo.evt = StateEvent.PROGRESS; // 카드 침.
                        }
                    }
                }
            }
        }
    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}