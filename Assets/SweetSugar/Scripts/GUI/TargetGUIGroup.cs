using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Target icons GUI handler. Appears on the top game panel 
    /// </summary>
    public class TargetGUIGroup : MonoBehaviour
    {
        public HorizontalLayoutGroup hg;
        public List<TargetGUI> list = new List<TargetGUI>();
        public TextMeshProUGUI description;
        HorizontalLayoutGroup group;
        //private bool panelGUI;
        //private bool levelLoaded;


        void OnEnable()
        {
            /*if (transform.root.name == "Level") 
                panelGUI = true;*/
            DisableImages();
            //levelLoaded = false;
            StartCoroutine(WaitForTarget());
            LevelManager.OnLevelLoaded += OnLevelLoaded;
            if (LevelManager.GetGameStatus() > GameState.PrepareGame)
                OnLevelLoaded();
        }

        private void DisableImages()
        {
            ClearTargets();
            description.gameObject.SetActive(false);
            foreach (var item in list)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {

            LevelManager.OnLevelLoaded -= OnLevelLoaded;

        }

        private void OnLevelLoaded()
        {
            group = GetComponent<HorizontalLayoutGroup>();
            if (group != null)
            {
                if (LevelData.THIS.IsTargetByNameExist("JellyBlock"))
                { group.spacing = 50; /*description.gameObject.SetActive(true);*/ }
                else
                { group.spacing = 0; /*description.gameObject.SetActive(false);*/ }

            }
            //levelLoaded = true;
        }

        IEnumerator WaitForTarget()
        {
            yield return new WaitUntil(() => LevelManager.THIS.levelLoaded);
            yield return new WaitUntil(() => LevelManager.THIS.levelData.GetTargetSprites().Length > 0);

            ClearTargets();
            SetTargets();
        }

        void SetTargets()
        {
            LevelData levelData = LevelManager.THIS.levelData;
            SetDescription(LevelManager.THIS.levelData.GetFirstTarget(true)?.GetDescription());
            var targets = levelData.GetTargetContainersForUI();
            if (transform.parent.parent.parent.name == "PreFailed")
            {
                targets = levelData.GetTargetCounters().Where(i => !i.IsTotalTargetReached()).ToArray();
            }
            for (var i = 0; i < targets.Length; i++)
            {
                var subTargetContainer = targets[i];
                list[i].SetSprite((Sprite) targets[i].extraObject);
                list[i].gameObject.SetActive(true);
                list[i].BindTargetGUI(subTargetContainer);
            }
        }

        private void SetDescription(string descr)
        {
            description.text = descr;
            if (descr != "")
            {
                // description.gameObject.SetActive(true);
                hg.padding.left = 58;
                hg.padding.right = 63;
            }
        }

        void ClearTargets()
        {
            hg.padding.left = 10;
            hg.padding.right = 10;

            description.gameObject.SetActive(false);
            // for (var i = 1; i < list.Count; i++)
            // {
            // Destroy(list[i].gameObject);
            // list.Remove(list[i]);
            // }
        }

        private void SetPadding()
        {
            if (list.Count == 2)
            {
                hg.padding.left = 150;
                hg.padding.right = 150;
            }

        }
    }
}
