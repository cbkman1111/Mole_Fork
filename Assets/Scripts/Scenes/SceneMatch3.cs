using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items._Interfaces;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.System.Orientation;
using SweetSugar.Scripts.System.Pool;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneMatch3 : SceneBase
{
    private LevelData levelData;
    private int currentLevel = 1;
    
    public SceneMatch3(SCENES scene) : base(scene)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init(JSONObject param)
    {
        UIMenuMatch3 menu = UIManager.Instance.OpenMenu<UIMenuMatch3>("UIMenuMatch3");
        if (menu != null)
        {
            menu.InitMenu();
            menu.OnStartGame = (int level) => {
                //LevelManager.THIS.gameStatus = GameState.PrepareGame;
                //GUIUtils.THIS.StartGame();
            };
        }
                
        IColorableComponent itemSprites;
        if (ObjectPooler.Instance != null) 
            itemSprites = ObjectPooler.Instance.GetPooledObject("Item", this, false).GetComponent<IColorableComponent>();

        // 레벨로드.
        currentLevel = 10;
        levelData = LoadingManager.LoadForPlay(currentLevel, levelData);
        /*
        levelData = new LevelData(Application.isPlaying, currentLevel);
        levelData = ScriptableLevelManager.LoadLevel(currentLevel);
        levelData.CheckLayers();
        levelData.LoadTargetObject();
        levelData.LoadTargetObject();
        levelData.InitTargetObjects(true);
        */

        // 필드 로드.
        var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>("GameBoard");
        var fieldBoards = new List<FieldBoard>();
        foreach (var fieldData in levelData.fields)
        {
            var _field = Instantiate(prefab);
            var fboard = _field.GetComponent<FieldBoard>();
            fboard.fieldData = fieldData;
            fboard.squaresArray = new Square[fieldData.maxCols * fieldData.maxRows];
            fieldBoards.Add(fboard);
        }

        // 필드 프리팹 생성.
        var fieldRoot = new GameObject("root");
        var fieldPos = new Vector3(-0.9f, 0, -10);
        var latestFieldPos = Vector3.right * ((fieldBoards.Count - 1) * 10) + Vector3.back * 10;
        var i = 0;
        foreach (var item in fieldBoards)
        {
            var _field = item.gameObject;
            _field.transform.SetParent(fieldRoot.transform);
            _field.transform.position = fieldPos + Vector3.right * (i * 15);
            var fboard = _field.GetComponent<FieldBoard>();

            fboard.CreateField();
            latestFieldPos = fboard.GetPosition();

            i++;
        }

        levelData.TargetCounters.RemoveAll(x => x.targetLevel.setCount == SetCount.FromLevel && x.GetCount() == 0);
        transform.position = latestFieldPos + Vector3.right * 10 + Vector3.back * 10;// - (Vector3)orientationGameCameraHandle.offsetFieldPosition;

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {

    }

    public override void OnTouchBean(Vector3 position)
    {

    }

    public override void OnTouchEnd(Vector3 position)
    {
       
    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}