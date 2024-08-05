using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;
using Gostop;
using UnityEditor;

namespace Scenes
{
    public class SceneGostop : SceneBase
    {
        [SerializeField]
        public Board board = null;

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

            board.Init(menu);
            return true;
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
                        if (board.StateInfo.state == State.HitCard)
                        {
                            if (board.MyTurn() == true)
                            {
                                var list = board.GetSameMonthCard((int)Board.Player.USER, card);
                                if (list.Count == 3)
                                {
                                    board.HitBomb((int)Board.Player.USER, list, card);
                                }
                                else if (list.Count == 4) // 총통
                                {
                                    board.HitChongtong((int)Board.Player.USER, list, card);
                                }
                                else
                                {
                                    board.HitCard((int)Board.Player.USER, card);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition){}
    }
}