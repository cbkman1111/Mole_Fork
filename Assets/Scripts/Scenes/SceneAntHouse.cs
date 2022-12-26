using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneAntHouse : SceneBase
{
    private Ant.Player player = null;
    private Ant.Joystick joystick = null;

    private Grid grid = null;
    private UIMenuAntHouse menu = null;
    

    public SceneAntHouse(SCENES scene) : base(scene)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init()
    {
        menu = UIManager.Instance.OpenMenu<UIMenuAntHouse>("UIMenuAntHouse");
        if (menu != null)
        {
            menu.InitMenu((Vector3 angle) => {
                OnMove(angle);
            });
        }

        var prefabMap = ResourcesManager.Instance.LoadInBuild<Grid>("Map_001");
        grid = Instantiate<Grid>(prefabMap);

        Vector3Int coordinate = new Vector3Int(0, -2, 0);

        var initPosition = grid.GetCellCenterLocal(coordinate);
        var prefabPlayer = ResourcesManager.Instance.LoadInBuild<Ant.Player>("Player");
        initPosition.z = 0;

        player = Instantiate<Ant.Player>(prefabPlayer);
        player.Init();
        player.transform.position = initPosition;

        var camera = AppManager.Instance.CurrScene.MainCamera;
        var position = camera.transform.position;
        position.x = player.transform.position.x;
        position.y = player.transform.position.y;

        camera.transform.position = position;
        /*
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition); //���� ���콺�� ��ġ�� Vector3�� ������
            Vector3Int gridPos = m_Grid.WorldToCell(world); //Vector3 position�� Vector3Int�� �ٲ���
          */

        return true;
    }

    private void OnMove(Vector3 angle)
    {
        player.Move(angle);
    }

    public void RemoveTile()
    {
        var position = player.GetHandPosition();

        //var world = MainCamera.ScreenToWorldPoint(position);
        var direction = transform.forward;

        var coordinate = grid.LocalToCell(position);

        var objects = grid.transform.Find("Objects");
        var tileMap = objects.GetComponent<UnityEngine.Tilemaps.Tilemap>();

        var sprite = tileMap.GetSprite(coordinate);
        var tile = tileMap.GetTile(coordinate);

        tileMap.SetTile(coordinate, null);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        Vector3 cameraPosition = MainCamera.transform.position;
        cameraPosition.x = player.transform.position.x;
        cameraPosition.y = player.transform.position.y;
        MainCamera.transform.position = cameraPosition;
    }

    public override void OnTouchBean(Vector3 position)
    {
        menu.Joystick.TouchBegin(position);
    }

    public override void OnTouchEnd(Vector3 position)
    {
        menu.Joystick.TouchEnd(position);

        if (EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }


        /*
        RaycastHit2D hit = Physics2D.Raycast(world, direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            Debug.DrawRay(world, direction * 10, Color.red, 0.3f);
        }
        else 
        {
            Debug.DrawRay(world, direction * 10, Color.blue, 0.3f);
        }
        */
    }

    public override void OnTouchMove(Vector3 position)
    {
        menu.Joystick.TouchMove(position);
    }
}