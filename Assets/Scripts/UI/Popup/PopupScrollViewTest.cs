using System.Collections.Generic;
using Common.UIObject;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    public class PopupScrollViewTest : PopupBase
    {
        private ScrollTest _scrollView = null;

        public RectTransform[] prefabs;
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        ///  
        /// </summary>
        public override void OnInit() 
        {
            base.OnInit();

            var list = new List<ScrollData>();
            for (var i = 0; i < 50; i++)
            {
                var data = new ScrollData();
                data.msg = $"[{i}] message~~~";
                data.type = (TestPrefabType)UnityEngine.Random.Range(0, 1);
                list.Add(data);
            }
    
            var trans = FindTransform(transform, "Scroll View - Test");
            if (trans != true) return;
            _scrollView = trans.GetComponent<ScrollTest>();
            _scrollView.Init(prefabs);
            _scrollView.SetItems(list);
        }

        private void Update()
        {
            SetText("Text - DeltaY", $"{_scrollView.CurrIndex}");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            transform.DOMove(
                    new Vector3(transform.position.x, Screen.height * 2f), 0.5f).
                SetEase(Ease.InOutExpo).
                OnComplete(() => {
                    base.Close();
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        protected override void OnClick(Button button)
        {
            var name = button.name;
            if (name == "Button - Ok")
            {
                Close();
            }
            else if (name == "Button - Insert")
            {
                //ScrollData data = new ScrollData();
                //data.name = $"insert message~~~";
                //scrollView.InsertMessage(data);
            }
        }
    }
}
