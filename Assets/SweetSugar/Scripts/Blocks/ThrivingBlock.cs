using System.Collections;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using UnityEngine;

namespace SweetSugar.Scripts.Blocks
{
    /// <summary>
    /// block expands constantly until you explode one
    /// </summary>
    public class ThrivingBlock : Square
    {
        static bool blockCreated;
        int lastMoveID = -1;
        public string Th;
        
        void OnEnable()
        {
            LevelManager.OnTurnEnd += OnTurnEnd;
            blockCreated = false;
        }

        //Added_feature
        public void RegisterToChocoSpawner(string key)
        {
            Th = key;
            if (LevelManager.THIS.thrivingBlockMachine)
            {
                if (LevelManager.THIS.IfanythrivingBlock.ContainsKey(Th))
                    LevelManager.THIS.IfanythrivingBlock[Th].Add(this);

            }
        }
        
        void OnDisable()
        {
            LevelManager.OnTurnEnd -= OnTurnEnd;
            LevelManager.THIS.thrivingBlockDestroyed = true;
            //Added_feature
            if (LevelManager.THIS.thrivingBlockMachine)
            {
                if (LevelManager.THIS.IfanythrivingBlock.ContainsKey(Th))
                    LevelManager.THIS.IfanythrivingBlock[Th].Remove(this);
            }
        }

        private void OnTurnEnd()
        {
            //Added_feature
            if (!LevelManager.THIS.thrivingBlockMachine)
            {
                if (LevelManager.THIS.moveID == lastMoveID) return;
                lastMoveID = LevelManager.THIS.moveID;
                if (LevelManager.THIS.thrivingBlockDestroyed || blockCreated ||
                    field != LevelManager.THIS.field) return;
                LevelManager.THIS.thrivingBlockDestroyed = false;
                var sqList = this.mainSquare.GetAllNeghborsCross();
                foreach (var sq in sqList)
                {
                    if (!sq.CanGoInto() || Random.Range(0, 1) != 0 ||
                        sq.type != SquareTypes.EmptySquare || sq.Item?.currentType != ItemsTypes.NONE) continue;
                    if (sq.Item == null) continue;
                    if (sq.Item.currentType != ItemsTypes.NONE) continue;
                    sq.CreateObstacle(SquareTypes.ThrivingBlock, 1);
                    blockCreated = true;
                    StartCoroutine(blockCreatedCD());
                    Destroy(sq.Item.gameObject);
                    break;
                }
                StartCoroutine(AI.THIS.CheckPossibleCombines());
            }
        }

        IEnumerator blockCreatedCD()
        {
            yield return new WaitForSeconds(1);
            blockCreated = false;
        }
    }
}