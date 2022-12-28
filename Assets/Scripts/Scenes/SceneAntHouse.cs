using Ant;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class SceneAntHouse : SceneBase
{
    private List<Monster> monsters = null;
    private Monster player = null;

    private Ant.Joystick joystick = null;

    private Grid grid = null;
    private UIMenuAntHouse menu = null;
    private NavMeshSurface surface = null;

    private MapData mapData = null;
    private PlayerData playerData = null;


    private int removeCount = 0;

    const string KEY_TILES = "tiles";

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

        Vector3Int coordinate = new Vector3Int(0, -2, 0);

        int mapId = 1;
        string mapName = $"Map_00{mapId}";
        var prefabMap = ResourcesManager.Instance.LoadInBuild<Grid>(mapName);
        grid = Instantiate<Grid>(prefabMap);

        var prefabBuilder = ResourcesManager.Instance.LoadInBuild<NavMeshSurface>("Builder");
        surface = Instantiate<NavMeshSurface>(prefabBuilder);
        surface.BuildNavMesh();

        /*
        monsers = new List<Ant.Monster>();
        for (int i = 0; i < 1; i++)
        {
            var monster = Instantiate<Ant.Monster>(prefabPlayer);
            monster.Init();
            monster.transform.position = initPosition;
            monster.name = $"monster_{i+1}";
            monsters.Add(monster);
        }
        */

        string key = MapData.GetKey(mapId);
        string data = PlayerPrefs.GetString(key);
        if (data != string.Empty)
        {
            mapData = JsonUtility.FromJson<MapData>(data);
        }
        else
        {
            mapData = new MapData(mapId);
        }

        key = PlayerData.GetKey();
        data = PlayerPrefs.GetString(key);
        if (data != string.Empty)
        {
            playerData = JsonUtility.FromJson<PlayerData>(data);
        }
        else
        {
            playerData = new PlayerData();
        }

        var prefabMonster = ResourcesManager.Instance.LoadInBuild<Monster>("Monster");
        player = Instantiate<Monster>(prefabMonster);
        if (player != null)
        {
            var initPosition = grid.GetCellCenterLocal(coordinate);

            player.GetComponent<NavMeshAgent>().enabled = false;
            player.Init(playerData);
            /*
            var camera = AppManager.Instance.CurrScene.MainCamera;
            var position = camera.transform.position;
            position.x = player.transform.position.x;
            position.y = player.transform.position.y;
            camera.transform.position = position;
            */
        }

        // 내가 보유한 타일 처리.
        var tileList = mapData.GetTiles();
        var walls = grid.transform.Find("Walls");
        var tileMap = walls.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        foreach (var tile in tileList)
        {
            tileMap.SetTile(tile.Cordinate, null);
        }

        surface.BuildNavMeshAsync();
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle"></param>
    public void OnMove(Vector3 angle)
    {
        player.Move(angle);
    }

    public void RemoveTile()
    {
        var position = player.GetHandPosition();
        var coordinate = grid.WorldToCell(position);
        coordinate.z = 0;

        var objects = grid.transform.Find("Walls");
        var tileMap = objects.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        var sprite = tileMap.GetSprite(coordinate);
        var tileInfo = tileMap.GetTile(coordinate);
        tileMap.SetTile(coordinate, null);

        var tile = new TileData(coordinate);
        mapData.Add(tile);
        mapData.Save();
        player.Save();

        removeCount++;

        //surface.BuildNavMeshAsync();
        //surface.BuildNavMesh();
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

        if (removeCount > 2)
        {
            removeCount = 0;
            surface.BuildNavMeshAsync();
            
        }
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

        if (monsters != null && monsters.Count > 0)
        {
            var world = MainCamera.ScreenToWorldPoint(position);
            //var direction = transform.forward;
            var coordinate = grid.WorldToCell(world);
            //var objects = grid.transform.Find("Objects");
            //var tileMap = objects.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            var pos = grid.GetCellCenterWorld(coordinate);

            //int rand = UnityEngine.Random.Range(0, monsters.Count);
            var agent = monsters[0].GetComponent<NavMeshAgent>();
            agent.SetDestination(pos);
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