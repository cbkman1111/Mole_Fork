using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.LeanTween.Framework;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// marmalade fly animation
    /// </summary>
    public class MarmaladeFly : UnityEngine.MonoBehaviour
    {
        public GameObject explosionPrefab;
        public GameObject particles;
        private Action callback;
        private bool reachedTarget;
        private int priority = 1;
        private bool canBeStarted;
        public ItemsTypes nextItemType;
        public bool setJelly;
        public Vector2Int[] targets;
        public float animationTime = 1.5f;
        private Vector3 pos, scale;
        private Quaternion rot;
        public int originSortingOrder = 2;
        public Vector3 startDirection;
        private ItemMarmalade thisItem;
        public IMarmaladeTargetable TargetItem;

        public void StartFly()
        {
            particles.SetActive(true);
            GetComponent<SpriteRenderer>().sortingLayerName = "ItemMask";
            GetComponent<SpriteRenderer>().sortingOrder = 10;
            canBeStarted = true;
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    var item = LevelManager.THIS.field.GetSquare(target.x, target.y).Item;
                    if (item.marmaladeTarget == null && TargetItem == null)
                    {
                        TargetItem = item;
                        TargetItem.GetMarmaladeTarget = gameObject;
//                SetAnimationKeys();
                    }
                }
            }
        }

        private void Awake()
        {
            thisItem = GetComponentInParent<ItemMarmalade>();
            pos = transform.localPosition;
            rot = transform.localRotation;
            scale = transform.localScale;
        }

        private void OnEnable()
        {
            particles.SetActive(false);
            TargetItem = null;
            setJelly = false;
            transform.localPosition = pos;
            transform.localRotation = rot;
            transform.localScale = scale;
        }

        private void OnDisable()
        {
            reachedTarget = false;
            LeanTween.Framework.LeanTween.cancel(gameObject);
            if (TargetItem != null) TargetItem.GetMarmaladeTarget = null;
            TargetItem = null;
        
            StopAllCoroutines();
            GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            GetComponent<SpriteRenderer>().sortingOrder = originSortingOrder;
            transform.localPosition = pos;
            transform.localRotation = rot;
            transform.localScale = scale;
        }


        private void ReachItem()
        {
            if (TargetItem != null && TargetItem.GetGameObject.activeSelf)
            {
                if (TargetItem.GetType().BaseType==typeof(Item))
                {
                    if (setJelly)
                        TargetItem.GetItem.square.SetType(SquareTypes.JellyBlock, 1, SquareTypes.NONE, 1);
                    reachedTarget = true;
                    callback?.Invoke();
                    if (nextItemType == ItemsTypes.NONE ||
                        (TargetItem.GetItem.currentType != ItemsTypes.NONE && (TargetItem.GetItem.currentType != ItemsTypes.MULTICOLOR ||
                                                                               LevelManager.THIS.AdditionalSettings.MulticolorDestroyByBoostAndMarmalade)) && TargetItem.GetItem.currentType != ItemsTypes.TimeBomb)
                    {
                        TargetItem.GetItem.DestroyItem(true, true, thisItem);
                    }
                    else
                    {
                        TargetItem.GetItem.NextType = nextItemType;
                        TargetItem.GetItem.ChangeType(item=>
                        {
                            if (item != null) item.DestroyItem();
                        },false);
                        TargetItem.GetItem.DestroyItem();
                    }

                    // targetItem.square.DestroyBlock();
                }
                else
                {
                    var square = TargetItem.GetGameObject.GetComponent<Square>();
                    square.DestroyBlock();
                    if (setJelly)
                        square.SetType(SquareTypes.JellyBlock, 1, SquareTypes.NONE, 1);
                    reachedTarget = true;
                    callback?.Invoke();
                }
            }

            DestroyMarmalade();
        }

        private void DestroyMarmalade()
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            LevelManager.THIS.FindMatches();
            gameObject.SetActive(false);
        }

        internal void SetDirection(Vector2 v)
        {
            Random.InitState(GetHashCode());
            var seq = LeanTween.Framework.LeanTween.sequence();
            seq.append(LeanTween.Framework.LeanTween.move(gameObject, transform.position - startDirection, animationTime ));
            seq.append(LeanTween.Framework.LeanTween.move(gameObject, LevelManager.THIS.field.GetPosition() + Random.insideUnitCircle*3 , animationTime).setOnComplete(() =>
            {
                if(gameObject.activeSelf)
                    StartCoroutine(FindTargetLoop());}));
            LeanTween.Framework.LeanTween.scale(gameObject, Vector3.one * 1.5f, animationTime );
            LeanTween.Framework.LeanTween.rotateAround(gameObject, Vector3.forward, 360, animationTime);
        }

        public void SecondPartDestroyAnimation(Action _callback)
        {
            callback = _callback;
            //MoveBack();
        }

        IEnumerator FindTargetLoop()
        {
            while (TargetItem == null)
            {
                FindTarget();
                yield return new WaitForSeconds(.01f);
            }
            LeanTween.Framework.LeanTween.cancel(gameObject);
            LeanTween.Framework.LeanTween.move(gameObject, TargetItem.GetGameObject.transform, 0.4f).setEase(LeanTweenType.easeInOutBack).setOnUpdate(CheckTargetUpdate).setOnComplete(ReachItem);
            LeanTween.Framework.LeanTween.scale(gameObject, Vector2.one*1.2f , 0.4f);
        }

        void CheckTargetUpdate(float f)
        {
            if ((TargetItem.GetItem?.destroying ?? false) || TargetItem == null || !TargetItem.GetGameObject.activeSelf)
            {
                StopCoroutine(FindTargetLoop());
                LeanTween.Framework.LeanTween.pause(gameObject);
                TargetItem = null;
                StartCoroutine(FindTargetLoop());
            }
        }

        private void FindTarget()
        {
            //Find hard to reach alone items or cages
            IEnumerable<IMarmaladeTargetable> items = new List<IMarmaladeTargetable>();
            //Check square targets
            if (!items.Any())
            {
                //looking for another item which is target of the current level 
                var targetContainer = LevelManager.THIS.levelData.GetFirstTarget(true);
                if (targetContainer != null)
                {
                    if (targetContainer.prefabs.FirstOrDefault()?.GetComponent<Square>() != null ||
                        targetContainer.prefabs.FirstOrDefault()?.GetComponent<LayeredBlock>() != null)
                        items = LevelManager.THIS.field.squaresArray.Where(i => i.type == (SquareTypes) Enum.Parse(typeof(SquareTypes), targetContainer.name))
                            .Select(i => i.Item).Where(i => !(i is null) && ArgItems(i)).MarmaladeCondition(gameObject, thisItem);

                    if (targetContainer.name == "Marshmello")
                    {
                        items = LevelManager.THIS.field.squaresArray.Where(i => i.Item != null && i.subSquares.Count > 1 && i.subSquares.Any(i => i.type == SquareTypes.BigBlock)).Select(i=>i.Item);
                    }
                }
            }         
            if (!items.Any())
                items = LevelManager.THIS.field.GetLonelyItemsOrCage()
                .Where(i => ArgItems(i));
            //If the marmalade should spread a jelly looking for non jellied squares
            if (setJelly)
                items = LevelManager.THIS.field.squaresArray.Where(i => i.type != SquareTypes.NONE && i.type != SquareTypes.JellyBlock).Select(i => i.Item).Where(i =>
                    i != null && i.Explodable).MarmaladeCondition(gameObject, thisItem);
            //trying to find ingredients
            if (!items.Any())
            {
                var list = LevelManager.THIS.field.squaresArray.Where(i => i.type != SquareTypes.NONE).Select(i => i.Item).Where(i =>
                    i != null && i.GetComponent<TargetComponent>() && !i.Combinable);
                if (list.Any())
                    items = list.Where(i=>i.square.nextSquare != null && i.square.nextSquare.Item && i.square.nextSquare.Item.Explodable).Select(i => i.square.nextSquare?.Item).MarmaladeCondition(gameObject, thisItem);
            }

 
            
            // //Looking for blocks
            if (items == null || !items.Any())
            {
                items = LevelManager.THIS.field.squaresArray.Where(i => i.IsObstacle() && i.IsHaveDestroybleObstacle() && i.GetSubSquare().type != SquareTypes.ChocoSpread);
            }
            //Looking through all items
            if (items == null || !items.Any())
            {
                items = LevelManager.THIS.field.GetItems().Where(i => i.Explodable).MarmaladeCondition(gameObject, thisItem);
            }

            TargetItem = items.MarmaladeCondition(gameObject, thisItem).OrderBy(i => Vector3.Distance(transform.position, i.GetGameObject.transform.position) - (float)(i.GetItem?.currentType??0)).FirstOrDefault();
            if (TargetItem != null && (Object)TargetItem != thisItem && (TargetItem.GetMarmaladeTarget == null || TargetItem.GetMarmaladeTarget == gameObject))
            {
                TargetItem.GetMarmaladeTarget = gameObject;
            }
        }

        private static bool ArgItems(Item i)
        {
            return i != null && i.Explodable && !i.needFall && !i.falling && !i.destroying && !i.JustCreatedItem;
        }

        public bool IsAnimationFinished()
        {
            return reachedTarget;
        }

        public int GetPriority()
        {
            return priority;
        }

        public bool CanBeStarted()
        {
            return canBeStarted;
        }
    }

    public static class MarmaladeUtils
    {
        public static IEnumerable<IMarmaladeTargetable> MarmaladeCondition(this IEnumerable<IMarmaladeTargetable> seq, GameObject gameObject, Item item)
        {
            return seq.WhereNotNull().Where(i => (i.GetMarmaladeTarget == null || i.GetMarmaladeTarget == gameObject)&& (Object)i != item && (!i.GetItem?.destroying ?? true));
        }  
    }
}