using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;
using Gostop;
using UnityEditor;
using System;
using TMPro;

namespace Scenes
{
    /// <summary>
    /// 고스톱 Scene 객체.
    /// </summary>
    public class SceneGostop : SceneBase
    {
        [SerializeField]
        public Board board = null;
        [SerializeField]
        private TextMeshPro[] Score = new TextMeshPro[(int)Board.Player.MAX];

        /// <summary>
        /// 씬 초기화.
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            UIMenuGostop menu = UIManager.Instance.OpenMenu<UIMenuGostop>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            board.Init(UpdateScore);
            board.StartGame();
            return true;
        }

        /// <summary>
        /// 점수 처리.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="score"></param>
        private void UpdateScore(Board.Player player, Score score)
        {
            int index = (int)player;
            Score[index].SetText($"{score.total} 점");
        }

        /// <summary>
        /// 화면 터치 때기.
        /// </summary>
        /// <param name="position"></param>
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
                        if (board.CommandInfo.type == Command.HitCard)
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