using Ant;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class SceneAntHouse : SceneBase
{
    private List<Monster> monsters = null;
    private Player player = null;

    private Joystick joystick = null;

    private Grid grid = null;
    private UIMenuAntHouse menu = null;
    private NavMeshSurface surface = null;

    private MapData mapData = null;
    private PlayerData playerData = null;
    private DateTime mapUpdateTime = DateTime.MinValue;

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
        monsters = new List<Monster>();
        mapUpdateTime = DateTime.MinValue;

        menu = UIManager.Instance.OpenMenu<UIMenuAntHouse>("UIMenuAntHouse");
        if (menu != null)
        {
            menu.InitMenu((Vector3 angle) => {
                OnMove(angle);
            });
        }

        int mapId = 3;
        string mapName = $"Map_00{mapId}";
        var prefabMap = ResourcesManager.Instance.LoadInBuild<Grid>(mapName);
        grid = Instantiate<Grid>(prefabMap);

        var prefabBuilder = ResourcesManager.Instance.LoadInBuild<NavMeshSurface>("Builder");
        surface = Instantiate<NavMeshSurface>(prefabBuilder);
        surface.BuildNavMesh();

        InitMapData(mapId);
        InitPlayer();
        InitMonster();
        InitTiles();

        StartCoroutine("BuildNavMesh", gameObject);
        return true;
    }

    protected IEnumerator BuildNavMesh()
    {
        while (true)
        {
            if (mapUpdateTime != DateTime.MinValue)
            {
                if (mapUpdateTime <= DateTime.Now)
                {
                    mapUpdateTime = DateTime.MinValue;
                    yield return surface.UpdateNavMesh(surface.navMeshData);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle"></param>
    public void OnMove(Vector3 angle)
    {
        player.Move(angle);
    }

    public void InitMapData(int mapId)
    {
        string key = MapData.GetKey(mapId);
        string data = PlayerPrefs.GetString(key);
        if (data != string.Empty)
        {
            mapData = JsonUtility.FromJson<MapData>(data);
        }
        else
        {
            var defaultPosition = grid.GetCellCenterLocal(new Vector3Int(0, 1, 0));

            mapData = new MapData(mapId);
            mapData.player.id = 0;
            mapData.player.position = defaultPosition;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void InitPlayer()
    {
        string key = PlayerData.GetKey();
        string data = PlayerPrefs.GetString(key);
        if (data != string.Empty)
        {
            playerData = JsonUtility.FromJson<PlayerData>(data);
        }
        else
        {
            playerData = new PlayerData() { 
                exp = 0,
                level = 0
            };
        }

        var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>("Monster");
        player = Monster.Create<Player>(prefab, mapData.player, enableAgent: false);
        if (player != null)
        {
            player.name = "monster_player";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void InitMonster()
    {
        var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>("Monster");
        var listMonster = mapData.GetMonsters();
        foreach (var monsterData in listMonster)
        {
            var monster = Monster.Create<AntQueen>(prefab, monsterData);
            if (monster != null)
            {
                monster.name = $"monster_{monsterData.id}";
                monsters.Add(monster);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void InitTiles()
    {
        // 내가 보유한 타일 처리.
        var tileList = mapData.GetTiles();
        var walls = grid.transform.Find("Walls");
        var tileMap = walls.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        foreach (var tile in tileList)
        {
            tileMap.SetTile(tile.Cordinate, null);
        }

        mapUpdateTime = DateTime.Now.AddSeconds(1);
    }

    public void CreateMonster()
    {
 
        var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>("Monster");
        
        ObjectData objData = new ObjectData();
        if (objData != null)
        {
            var defaultPosition = grid.GetCellCenterLocal(new Vector3Int(0, 1, 0));

            objData.id = monsters.Count;
            objData.position = defaultPosition;
            mapData.AddMonster(objData);

            var monster = Monster.Create<Monster>(prefab, objData);
            if (monster != null)
            {
                monsters.Add(monster);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
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
        mapData.AddTile(tile);
        mapUpdateTime = DateTime.Now.AddSeconds(5);
    }

    public void SaveGame()
    {
        player.UpdateData();
        monsters.ForEach(m => m.UpdateData());

        mapData.Save();
        playerData.Save();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public override void OnTouchBean(Vector3 position)
    {
        menu.Joystick.TouchBegin(position);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
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
            var coordinate = grid.WorldToCell(world);
            var pos = grid.GetCellCenterWorld(coordinate);
            int rand = UnityEngine.Random.Range(0, monsters.Count);
            monsters[rand].SetDestination(pos);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public override void OnTouchMove(Vector3 position)
    {
        menu.Joystick.TouchMove(position);
    }
}