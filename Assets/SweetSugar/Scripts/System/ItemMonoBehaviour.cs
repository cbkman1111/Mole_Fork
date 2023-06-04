using System.Collections;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.System.Pool;
using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public class ItemMonoBehaviour : MonoBehaviour
    {
        //private bool quit;

        protected virtual void Start()
        {
            // Debug.Log(gameObject.name + " " + gameObject.GetInstanceID() + " created ");
        }

        public void DestroyBehaviour()
        {
            var item = GetComponent<Item>();
            if (item == null || !gameObject.activeSelf) return;
            StartCoroutine(DestroyDelay(item));
        }

        public static int finishBonusCounter;

        IEnumerator DestroyDelay(Item item)
        {
            bool changeTypeFinished=false;
            if (item.NextType != ItemsTypes.NONE)
            {
                if(item.currentType != ItemsTypes.PACKAGE)
                    yield return new WaitWhile(() => item.falling);
                if(LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
                {
                    finishBonusCounter++;
                    if (finishBonusCounter >= 5) item.NextType = ItemsTypes.NONE;
                }
                if(LevelManager.THIS.gameStatus != GameState.PreWinAnimations || (LevelManager.THIS.gameStatus == GameState.PreWinAnimations && finishBonusCounter <5))
                {
                    item.ChangeType((x) => { changeTypeFinished = true; });
                    yield return new WaitUntil(() => changeTypeFinished);
                }
            }

            if (LevelManager.THIS.DebugSettings.DestroyLog)
                DebugLogKeeper.Log(name + " dontDestroyOnThisMove " + item.dontDestroyOnThisMove + " dontDestroyForThisCombine " + gameObject.GetComponent<Item>()
                                       .dontDestroyForThisCombine,  DebugLogKeeper.LogType.Destroying);
            if (item.dontDestroyOnThisMove || gameObject.GetComponent<Item>().dontDestroyForThisCombine)
            {
                GetComponent<Item>().StopDestroy();
                yield break;
            }
            if (LevelManager.THIS.DebugSettings.DestroyLog)
                DebugLogKeeper.Log(gameObject.GetInstanceID() + " destroyed " + item.name + " " + item.GetInstanceID(), DebugLogKeeper.LogType.Destroying);
            OnDestroyItem(item);
            ObjectPooler.Instance.PutBack(gameObject);
            yield return new WaitForSeconds(0);
        }

        private void OnDestroyItem(Item item)
        {
//        if (item.square && item == item.square.Item)
//            item.square.Item = null;
            item.square = null;
            item.field.squaresArray.Where(i => i.Item == item).ForEachY(i => i.Item = null);
            item.previousSquare = null;
            item.tutorialItem = false;
            item.NextType = ItemsTypes.NONE;
            if(transform.childCount>0)
            {
                transform.GetChild(0).transform.localScale = Vector3.one;
                transform.GetChild(0).transform.localPosition = Vector3.zero;
            }
        }

        void OnApplicationQuit()
        {
            //quit = true;
        }

        void OnDestroy()
        {
            // if (!quit) Debug.Log(gameObject.name + " " + gameObject.GetInstanceID() + " OnDestroyed ");
        }
    }
}