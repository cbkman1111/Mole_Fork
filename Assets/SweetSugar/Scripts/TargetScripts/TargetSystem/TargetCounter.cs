using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetEditor;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    public class TargetCounter
    {
          public GameObject targetPrefab;

        public int count;
        int Savecount;
        public int preCount;
        public Sprite extraObject;
        public Sprite[] extraObjects;
        public int color;
        public TargetGUI TargetGui;
        public TargetContainer targetLevel;
        public CollectingTypes collectingAction;
        public bool NotFinishUntilMoveOut;
        private bool showScores;
        private int countALlsquares;

        public TargetCounter(GameObject _target, int _count, Object[] _extraObject, int color, TargetContainer argTargetLevel, bool _NotFinishUntilMoveOut,
            TargetObject targetObject)
        {
            targetPrefab = _target;
            count = _count;
            if(argTargetLevel.name == "Stars" && targetObject.ShowTheScoreForStar.ShowTheScore)
            {
                showScores = targetObject.ShowTheScoreForStar.ShowTheScore;
                if (targetObject.CountDrawer.count == 1) count = LevelData.THIS.star1; 
                if (targetObject.CountDrawer.count == 2) count = LevelData.THIS.star2; 
                if (targetObject.CountDrawer.count == 3) count = LevelData.THIS.star3; 
            }
            targetLevel = argTargetLevel;
            extraObjects = (Sprite[]) _extraObject;
            if (extraObjects != null && extraObjects.Length > 0)
            {
                var uiSprite = targetObject.sprites.FirstOrDefault(i => i.uiSprite == true);
                extraObject = extraObjects[0];
                if (uiSprite != null) extraObject = uiSprite.icon;
            }
            count = GetCountForSquares(false);
            NotFinishUntilMoveOut = _NotFinishUntilMoveOut;
            Savecount = count;
            preCount = count;
            this.color = color;
            collectingAction = argTargetLevel.collectAction;
        }

        private int GetCountForSquares(bool subLevel)
        {
            if (targetLevel != null && targetLevel.setCount == TargetSystem.SetCount.FromLevel)
            {
                if (targetLevel.collectAction == CollectingTypes.Destroy || targetLevel.collectAction == CollectingTypes.Clear)
                {
                    count = GetSquareTargetCount(false);
                    return GetSquareTargetCount(subLevel);
                }
                if (targetLevel.collectAction == CollectingTypes.Spread) return GetSquareSpreadCount(subLevel);
            }
            return count;
        }

        private int GetSquareSpreadCount(bool subLevel)
        {
            int countJelly;
            countALlsquares = LevelManager.THIS.fieldBoards.Sum(i => i.GetSquares().Where(x => x.IsAvailable()).Count());
            int countALl;
            if(!subLevel)
            {
                countJelly = LevelManager.THIS.fieldBoards.Sum(i => i.GetSquares().Sum(x => x.GetSpriteRenderers().Distinct().Sum(y => extraObjects.Count(z => z == y.sprite))));
                countALl = LevelManager.THIS.fieldBoards.Sum(i => i.GetSquares().Where(x => x.IsAvailable()).Count());
            }
            else
            {
                countJelly = LevelManager.THIS.field.GetSquares().Sum(x => x.GetSpriteRenderers().Sum(y => extraObjects.Count(z => z == y.sprite)));
                countALl = LevelManager.THIS.field.GetSquares().Where(x => x.IsAvailable()).Count();
            }
            return countALl - countJelly;
        }
        
        private int GetSquareTargetCount(bool subLevel)
        {
            if(!subLevel)
                return GameObject.FindObjectsOfType<TargetComponent>().Sum(x => x.GetComponent<ISquareItemCommon>().GetSpriteRenderers().Sum(y => extraObjects.Count(z => z == y.sprite)));
            return GameObject.FindObjectsOfType<TargetComponent>().Where(i=>i.GetComponent<IField>().GetField()==LevelManager.THIS.field).Sum(x => x.GetComponent<ISquareItemCommon>()
                .GetSpriteRenderers().Sum(y => extraObjects.Count(z => z == y.sprite)));
        }

        public void OnCheckTarget(GameObject obj, Sprite spr)
        {
            var item = obj.GetComponent<ISquareItemCommon>();
            var spriteRenderers = item.GetSpriteRenderers();
            // var b = obj.GetComponent<Square>();
            // if (b) spriteRenderers = b.GetSubSquare().GetSpriteRenderers();
            if (collectingAction == CollectingTypes.ReachBottom && !item.IsBottom()) return;
            var objects = spriteRenderers.Where(x => extraObjects.Any(i => i == x.sprite)).ToArray();
            if (objects.Count() > 0 && count > 0)
            {
                var animCompLinkObject = objects.OrderByDescending(i=>i.sortingOrder).First().gameObject;
                var square = animCompLinkObject.GetComponent<Square>();
                if (LevelManager.THIS.animateItems.Any(i => square != null && i.linkObjectHash == square.hashCode)) return;
                // if(square)
                // {
                //     if (square.destroyedTarget) return;
                //     square.destroyedTarget = true;
                // }
                Vector3 pos = TargetGUI.GetTargetGUIPosition(extraObject.name);
                var itemAnim = new GameObject();
                var animComp = itemAnim.AddComponent<AnimateItems>();
                LevelManager.THIS.animateItems.Add(animComp);
                if (square != null) animComp.linkObjectHash = square.hashCode;
                animComp.target = true;
                animComp.InitAnimation(animCompLinkObject, pos, obj.transform.localScale, () => { 
                    // if(square != null)
                    // {
                    //     var component = square.transform.parent.GetComponent<Square>();
                    //     component.destroyIteration = 0;
                    //     component.DestroyBlock();
                    // }
                    changeCount(-1); 
                });
            }

            if (targetLevel.name == "Stars")
            {
                if(!showScores) count = Savecount - LevelManager.THIS.stars;
                else count = Savecount - LevelManager.Score;
                if (count < 0) count = 0;
            }
        }

        public void changeCount(int i)
        {
            count += i;
            if (count < 0) count = 0;
            preCount = count;
            if(!LevelManager.THIS.DragBlocked && count == 0) LevelManager.THIS.CheckWinLose();
        }

        public int GetCount(bool game=false)
        {
            if(!IsTargetStars() && !game)
            {
                var countForSquares = GetCountForSquares(false);
                if (countForSquares < 0) countForSquares = 0;
                return countForSquares;
            }
            else if (!IsTargetStars() && game)
            {
                if (targetLevel.collectAction == CollectingTypes.Spread) return countALlsquares - count;
                return count;
            }

            return count;
        }

        public virtual bool IsTargetReachedSublevel()
        {
            if (targetLevel.CanSwithSublevel) return true;
            if (targetLevel.collectAction == CollectingTypes.ReachBottom && LevelManager.THIS.field.IngredientsByEditor)
            {
                if (LevelManager.THIS.field.GetItems().Count(i => i.currentType == ItemsTypes.INGREDIENT) == 0) return true;
            }
            return GetCountForSquares(true) <= 0;
        }

        public virtual bool IsTotalTargetReached()
        {
            return GetCountForSquares(false) <= 0;
        }

        public void BindGUI(TargetGUI targetGui)
        {
            TargetGui = targetGui;
            if (targetLevel.name == "Stars" && !showScores)
            {
                var stars = GameObject.Find("ProgressBar").transform.Find("Stars");
                var star = stars.GetChild(Savecount - 1).gameObject;
                LeanTween.Framework.LeanTween.delayedCall(star, 5f, () => { LeanTween.Framework.LeanTween.scale(star, Vector3.one * 1.2f, 0.4f).setLoopPingPong().setRepeat(4); }).setRepeat(-1);
            }
        }

        public string GetTargetName() => targetLevel.name;

        public bool IsTargetStars() => GetTargetName() == "Stars";

        public void CheckTarget(Item[] items)
        {
                foreach (var item in items)
                {
                    if (item != null)
                        OnCheckTarget(item.gameObject, item.GetSprite());
                }
        }

        public void CheckTarget(Square[] squares, bool afterDestroy=true)
        {
            if(targetLevel.collectAction == CollectingTypes.Spread)
            {
                if (squares.SelectMany(i => i.GetSpriteRenderers().Select(x=>x.sprite)).Any(i=>extraObjects.Any(x=>x==i)))
                {
                    foreach (var item in squares)
                    {
                        if (item != null && item.type == SquareTypes.EmptySquare) item.SetType(SquareTypes.JellyBlock, 1, SquareTypes.NONE, 1);
                    }
                }
            }
            else if (targetLevel.collectAction == CollectingTypes.Clear && !afterDestroy)
            {
                var obj = Object.FindObjectsOfType<TargetComponent>().Where(i => extraObjects.Any(x =>
                {
                    var spriteRenderer = i.GetComponent<SpriteRenderer>();
                    return spriteRenderer != null && x == spriteRenderer.sprite;
                }));
                foreach (var component in obj)
                {
                    if(component.GetComponent<Square>().IsCleared())
                    {
                        OnCheckTarget(component.gameObject, component.GetComponent<Square>().GetSprite());
                    }
                }
            }
            else if (targetLevel.collectAction != CollectingTypes.Clear)
                foreach (var item in squares)
                    OnCheckTarget(item.gameObject, item.GetSprite());
        }

        public void CheckBottom()
        {
            var sqList = LevelManager.THIS.field.GetBottomRow();
                foreach (var hItem in sqList)
                {
                    if (hItem.Item == null || hItem.Item.falling) continue;
                    var obj = hItem.Item;
                    var sprName = obj.GetSprite().name;
                    if(extraObjects.Any(i=>i.name == sprName))
                    {
                        OnCheckTarget(obj.gameObject, obj.GetSprite());
                        obj.DestroyBehaviour();
                    }
                }
        }
    }
}