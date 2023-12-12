using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneGostop : SceneBase
    {
        private Board _board = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            UIMenuGostop menu = UIManager.Instance.OpenMenu<UIMenuGostop>("UIMenuGostop");
            if (menu != null)
            {
                menu.InitMenu();
            }

            _board = Board.Create(menu);
            if (_board != null)
            {
                _board.StartGame();
            }

            return true;
        }

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
                        var stateMachine = _board.GetStateMachine();
                        var turnInfo = stateMachine.GetCurrturnInfo();
                        var stateInfo = turnInfo.GetCurrentStateInfo();

                        if (stateInfo.state == State.CARD_HIT &&
                            _board.MyTurn() == true)
                        {
                            var list = _board.GetSameMonthCard((int)Board.Player.USER, card);
                            if (list.Count == 3)
                            {
                                _board.HitBomb((int)Board.Player.USER, list, card);
                                stateInfo.evt = StateEvent.PROGRESS; 
                            }
                            else if (list.Count == 4) // 총통
                            {
                                _board.HitChongtong((int)Board.Player.USER, list, card);
                                stateInfo.evt = StateEvent.PROGRESS;
                            }
                            else 
                            {
                                _board.HitCard((int)Board.Player.USER, card);
                                stateInfo.evt = StateEvent.PROGRESS; 
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
}