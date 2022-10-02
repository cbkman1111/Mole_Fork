using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuGostop : MenuBase
{
    private Board board = null;

    public override void OnInit()
    {

    }

    public bool InitMenu(Board board)
    {
        this.board = board;

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

        ShowScoreMenu(false);
        return true;
    }

    public void ShowScoreMenu(bool active)
    {
        SetActive("Image - Gwang Player 1", active);
        SetActive("Image - Mung Player 1", active);
        SetActive("Image - Thee Player 1", active);
        SetActive("Image - Pee Player 1", active);

        SetActive("Image - Gwang Player 2", active);
        SetActive("Image - Mung Player 2", active);
        SetActive("Image - Thee Player 2", active);
        SetActive("Image - Pee Player 2", active);
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if(name == "Button - Exit")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.MENU);
        }
    }
}
