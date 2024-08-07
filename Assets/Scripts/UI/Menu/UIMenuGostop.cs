using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;
using Gostop;

namespace UI.Menu
{
    public class UIMenuGostop : MenuBase
    {
        public bool InitMenu()
        {
            ShowScoreMenu(false);
            
            var slider = GetObject<Slider>("Slider - TimeScale");
            if(slider != null)
                slider.value = Time.timeScale;

            return true;
        }

        public void SetPosition(Board board)
        {
            var cam = AppManager.Instance.CurrScene.MainCamera;

            Vector3 gwang = cam.WorldToScreenPoint(board.boardPositions[0].GwangScore.position);
            Vector3 mung = cam.WorldToScreenPoint(board.boardPositions[0].MungScore.position);
            Vector3 thee = cam.WorldToScreenPoint(board.boardPositions[0].TheeScore.position);
            Vector3 pee = cam.WorldToScreenPoint(board.boardPositions[0].PeeScore.position);

            SetPosition("Image - Gwang Player 1", gwang);
            SetPosition("Image - Mung Player 1", mung);
            SetPosition("Image - Thee Player 1", thee);
            SetPosition("Image - Pee Player 1", pee);

            gwang = cam.WorldToScreenPoint(board.boardPositions[1].GwangScore.position);
            mung = cam.WorldToScreenPoint(board.boardPositions[1].MungScore.position);
            thee = cam.WorldToScreenPoint(board.boardPositions[1].TheeScore.position);
            pee = cam.WorldToScreenPoint(board.boardPositions[1].PeeScore.position);

            SetPosition("Image - Gwang Player 2", gwang);
            SetPosition("Image - Mung Player 2", mung);
            SetPosition("Image - Thee Player 2", thee);
            SetPosition("Image - Pee Player 2", pee);
        }

        public void ShowScoreMenu(bool active)
        {
            /*
            SetActive("Image - Gwang Player 1", active);
            SetActive("Image - Mung Player 1", active);
            SetActive("Image - Thee Player 1", active);
            SetActive("Image - Pee Player 1", active);

            SetActive("Image - Gwang Player 2", active);
            SetActive("Image - Mung Player 2", active);
            SetActive("Image - Thee Player 2", active);
            SetActive("Image - Pee Player 2", active);
            */
        }

        public void ScoreUpdate(Board.Player user, Score score)
        {
            int userIndex = 1;
            if (user == Board.Player.COMPUTER)
            {
                userIndex = 2;
            }

            SetText($"Text - Gwang Player {userIndex}", $"{score.gawng}");
            SetText($"Text - Mung Player {userIndex}", $"{score.mung}");
            SetText($"Text - Thee Player {userIndex}", $"{score.thee}");
            SetText($"Text - Pee Player {userIndex}", $"{score.pee}");

            SetText($"Text - Total Score Player {userIndex}", $"{score.total}");
        }

        public void SetDebug(string msg)
        {
            ScrollRect scroll = GetObject<ScrollRect>("Scroll View - Debug");
            if (scroll != null)
            {
                var textDebug = scroll.content.GetComponent<Text>();
                SetText(textDebug, msg);

                scroll.verticalNormalizedPosition = 0f;
            }
        }

        public void SetDebugTrun()
        { 

        }

        public override void OnValueChanged(Slider slider, float f)
        {
            //base.OnValueChanged(slider, f);
            if (slider.name == "Slider - TimeScale")
            {
                Time.timeScale = f;
            }
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if(name == "Button - Exit")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }

        public override void OnInit() { }
    }
}
