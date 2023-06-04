using System.Collections;
using System.Collections.Generic;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using UnityEngine;

namespace SweetSugar.Scripts.Spawner
{
    public class ChocoSpawner : Square
    {
        //static bool blockCreated;
        int lastMoveID = -1;
        Square NeghborSquare;
        string SpawnerKey = "";

        void OnEnable()
        {
            GetComponent<SpriteRenderer>().sortingOrder = 5;
            SpawnerKey = LevelManager.THIS.SubScribeChocoSpread();
            NeghborSquare = this;
            LevelManager.OnTurnEnd += OnTurnEnd;
            LevelManager.THIS.thrivingBlockMachine = true;
        }

        void OnDisable()
        {
            LevelManager.THIS.UnSubscribeChocoSpread(SpawnerKey);
            LevelManager.OnTurnEnd -= OnTurnEnd;
            LevelManager.THIS.thrivingBlockDestroyed = true;
            LevelManager.THIS.thrivingBlockMachine = false;
        }

        private void OnTurnEnd()
        {
            if(SpawnerKey == "1")
            {
                LevelManager.THIS.GenerateRandom();
            }

            if (LevelManager.THIS.Getint.ToString() != SpawnerKey) return;

            if (LevelManager.THIS.moveID == lastMoveID) return;
            lastMoveID = LevelManager.THIS.moveID;
            if (LevelManager.THIS.thrivingBlockDestroyed /*|| blockCreated*/ || field != LevelManager.THIS.field) return;
            LevelManager.THIS.thrivingBlockDestroyed = false;
            List<Square> sqList = new List<Square>();
            if (NeghborSquare == this || (LevelManager.THIS.IfanythrivingBlock[SpawnerKey].Count == 0))
            {
                sqList = this.mainSquare.GetAllNeghborsCross();
            }
            else
            {
                var res = TargetSquare();
                if(res != null)
                    NeghborSquare = res;
                else
                {
                    Debug.Log("Got Null");
                }
                sqList = NeghborSquare.GetAllNeghborsCross();
            }
            foreach (var sq in sqList)
            {
                if (!sq.CanGoInto() || Random.Range(0, 1) != 0 ||
                    sq.type != SquareTypes.EmptySquare || sq.Item?.currentType != ItemsTypes.NONE) continue;
                if (sq.Item == null) continue;
                var SampleType = sq.Item.currentType;
                if (SampleType == ItemsTypes.INGREDIENT || SampleType == ItemsTypes.SPIRAL  || SampleType == ItemsTypes.TimeBomb ) continue;
                sq.CreateObstacle(SquareTypes.ThrivingBlock, 1);
                var th = sq.GetComponentInChildren<ThrivingBlock>();
                if (th != null)
                {
                    th.RegisterToChocoSpawner(SpawnerKey);
                }
                //blockCreated = true;
                StartCoroutine(blockCreatedCD());
                NeghborSquare = sq;

                Destroy(sq.Item.gameObject);
                sq.item = null;
                break;
            }
        }

        private Square TargetSquare()
        {
            Square res = null;

            foreach (var item in LevelManager.THIS.IfanythrivingBlock[SpawnerKey])
            {
                foreach (var item1 in item.mainSquare.GetAllNeghborsCross())
                {
                    if(item1.Item != null && item1.Item.currentType == ItemsTypes.NONE)
                    {
                        res = item.mainSquare;
                        break;
                    }
                }
            }


            return res;
        }

        IEnumerator blockCreatedCD()
        {
            yield return new WaitForSeconds(1);
            //blockCreated = false;
        }
    }
}   
