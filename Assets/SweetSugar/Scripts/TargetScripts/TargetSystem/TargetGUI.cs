using System.Collections;
using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// target icon
    /// </summary>
    public class TargetGUI : MonoBehaviour
    {
        public Image image;
        List<Sprite> targetSprites = new List<Sprite>();
        public TextMeshProUGUI text;
        public GameObject check;
        public GameObject uncheck;
        public int color;
        public TargetCounter subTargetContainer;

        public void SetSprite(Sprite spr)
        {
            image.sprite = spr;
        }

        void OnEnable()
        {
            // if (LevelData.THIS?.target.name == "Stars")
            // {
            //     gameObject.SetActive(false);
            //     return;
            // }
            check.SetActive(false);
            uncheck.SetActive(false);
            // UpdateText();
            StartCoroutine(Check());
        }

        private void Update()
        {
            if(subTargetContainer != null)
                UpdateText();
        }

        private void UpdateText()
        {
            var count = subTargetContainer.GetCount(true);
            // var count2 = LevelManager.THIS.fieldBoards.Sum(i => i.GetSquares().Where(x => x.IsAvailable()).Count())-  LevelManager.THIS.fieldBoards.SelectMany(i => i.GetSquares()).Count(i => i.type == SquareTypes.JellyBlock); //subTargetContainer.GetCount(false);
            text.text = count.ToString();
            // if (count != count2 && subTargetContainer.collectingAction == CollectingTypes.Spread)
            // {
                // Debug.Log(count + " != " + count2);
                // Debug.Break();
            // }
        }

        IEnumerator Check()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (text.text == "0")
                {
                    check.SetActive(true);
                    uncheck.SetActive(false);
                    text.GetComponent<TextMeshProUGUI>().enabled = false;
                }
                else if (LevelManager.THIS.gameStatus == GameState.PreFailed || LevelManager.THIS.gameStatus == GameState.GameOver)
                {
                    check.SetActive(false);
                    uncheck.SetActive(true);
                    text.GetComponent<TextMeshProUGUI>().enabled = false;

                }
                else
                {
                    check.SetActive(false);
                    uncheck.SetActive(false);
                    text.GetComponent<TextMeshProUGUI>().enabled = true;

                }
            }
        }

        public static Vector2 GetTargetGUIPosition(string SpriteName)
        {
            var pos = Vector2.zero;
            var list = FindObjectsOfType(typeof(TargetGUI)) as TargetGUI[];
            foreach (var item in list)
            {
                if (item.image.GetComponent<Image>().sprite.name == SpriteName && item.gameObject.activeSelf)
                    return item.transform.position;
            }
            if (list.Length > 0) pos = list[0].transform.position;
            return pos;
        }
        
        public static Vector2 GetTargetGUIPosition(int color)
        {
            var pos = Vector2.zero;
            var list = FindObjectsOfType(typeof(TargetGUI)) as TargetGUI[];
            foreach (var item in list)
            {
                if (item.color == color && item.gameObject.activeSelf)
                    return item.transform.position;
            }
            if (list.Length > 0) pos = list[0].transform.position;
            return pos;
        }

        public void BindTargetGUI(TargetCounter subTarget)
        {
            subTargetContainer = subTarget;
            subTargetContainer.BindGUI(this);
            if (subTargetContainer.extraObject != null) color = subTargetContainer.color;
            text.text = subTargetContainer.GetCount().ToString();
        }
    }
}
