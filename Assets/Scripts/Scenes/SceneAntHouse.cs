using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneAntHouse : SceneBase
{
    private Ant.Player player = null;
    private Grid grid = null;

    public SceneAntHouse(SCENES scene) : base(scene)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init()
    {
        UIMenuAntHouse menu = UIManager.Instance.OpenMenu<UIMenuAntHouse>("UIMenuAntHouse");
        if (menu != null)
        {
            menu.InitMenu();
        }


        var prefabMap = ResourcesManager.Instance.LoadInBuild<Grid>("Map_001");
        grid = Instantiate<Grid>(prefabMap);

        Vector3Int coordinate = new Vector3Int(0, -2, 0);

        var initPosition = grid.GetCellCenterLocal(coordinate);
        var prefabPlayer = ResourcesManager.Instance.LoadInBuild<Ant.Player>("Player");
        initPosition.z = 0;

        player = Instantiate<Ant.Player>(prefabPlayer);
        player.transform.position = initPosition;

        var camera = AppManager.Instance.CurrScene.MainCamera;
        var position = camera.transform.position;
        position.x = player.transform.position.x;
        position.y = player.transform.position.y;

        camera.transform.position = position;
        /*
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition); //현재 마우스의 위치를 Vector3로 가져옴
            Vector3Int gridPos = m_Grid.WorldToCell(world); //Vector3 position을 Vector3Int로 바꿔줌
          */

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
        var world = MainCamera.ScreenToWorldPoint(position);
        var direction = transform.forward;

        var coordinate = grid.LocalToCell(world);

        var objects = grid.transform.FindChild("Objects");
        var tileMap = objects.GetComponent<UnityEngine.Tilemaps.Tilemap>();

        var sprite = tileMap.GetSprite(coordinate);
        var tile = tileMap.GetTile(coordinate);

        tileMap.SetTile(coordinate, null);
        
        RaycastHit2D hit = Physics2D.Raycast(world, direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            Debug.DrawRay(world, direction * 10, Color.red, 0.3f);
        }
        else 
        {
            Debug.DrawRay(world, direction * 10, Color.blue, 0.3f);
        }
    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}