using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common.Global;
using Common.Scene;
using Common.Utils.Pool;
using DG.Tweening;
using Games.TileMap;
using Games.TileMap.Datas;
using TileMap;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneTileMap : SceneBase
    {
        private static readonly System.Random random = new System.Random();
        private static readonly object synlock = new object();

        public MapData MapData => _mapData;
        private MapData _mapData = null;
        private bool gameLoaded = false;
        
        private UIMenuTileMap _menu = null;
        private Map _map = null;
        private Pige _pigeon = null;

        private float _ditanceCamera = 0;

        private MEC.CoroutineHandle handler;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            _ditanceCamera = 5;
            
            _menu = UIManager.Instance.OpenMenu<UIMenuTileMap>("UI/UIMenuTileMap");
            if (_menu != null)
            {
                _menu.InitMenu(
                    move: (Vector3 angle) => {
                        OnMove(angle);
                    },
                    stop: () => {
                        OnStop();
                    },
                    save:() =>
                    {
                        _mapData.X = _pigeon.x;
                        _mapData.Z = _pigeon.z;
                        
                        _mapData.Save();
                    },
                    zoom: (float percent) =>
                    {
                        float amount = 6 * percent;
                        _ditanceCamera = 5 + amount;
                    });
            }

            PoolManager.Instance.InitList(
                ResourcesManager.Instance.LoadInBuild<Transform>("TileGround"),
                ResourcesManager.Instance.LoadInBuild<Transform>("TileWater"),
                
                ResourcesManager.Instance.LoadInBuild<Transform>("Bush"),
                ResourcesManager.Instance.LoadInBuild<Transform>("PineTree"),
                ResourcesManager.Instance.LoadInBuild<Transform>("Sapling"),
                ResourcesManager.Instance.LoadInBuild<Transform>("SeaWeed"),
                
                ResourcesManager.Instance.LoadInBuild<Transform>("Mushroom_empty"),
                ResourcesManager.Instance.LoadInBuild<Transform>("Mushroom_green"),
                ResourcesManager.Instance.LoadInBuild<Transform>("Mushroom_red"),
                ResourcesManager.Instance.LoadInBuild<Transform>("Mushroom_seed"),
                ResourcesManager.Instance.LoadInBuild<Transform>("Mushroom_sky"),

                ResourcesManager.Instance.LoadInBuild<Transform>("Bee"),
                ResourcesManager.Instance.LoadInBuild<Transform>("Spider"),
                ResourcesManager.Instance.LoadInBuild<Transform>("FishMan"),
                ResourcesManager.Instance.LoadInBuild<Transform>("FishA"));
            
            var startX = _mapData.X;
            var startZ = _mapData.Z;

            // 맵 생성.
            const int displayW = 13;
            const int displayUpSide = 12;
            const int displayDownSide = 5;
            var prefabMap = ResourcesManager.Instance.LoadInBuild<Map>("Map");
            _map = Object.Instantiate<Map>(prefabMap);
            _map.transform.position = Vector3.zero;
            _map.Init(_mapData, startX, startZ, displayW, displayUpSide, displayDownSide);
            
            // 캐릭터 생성.
            var prefab = ResourcesManager.Instance.LoadInBuild<Pige>("PigeonTemp");
            _pigeon = Object.Instantiate<Pige>(prefab);
            _pigeon.Init(startX, startZ);

            // 카메라 위치 초기화.
            MainCamera.transform.position = _pigeon.transform.position + /*new Vector3(0, 1, 0) + */
                                            _pigeon.transform.GetChild(0).forward * -_ditanceCamera;
            return true;
        }

        /// <summary>
        /// Load 쓰레드에서 사용할 랜덤. 기존거는 메인 쓰레드만 된다.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomNumber(int min, int max)
        {
            lock (synlock)
            {
                return random.Next(min, max);
            }
        }
        
        /// <summary>
        /// 저장된 데이터가 있으면 로드.
        /// </summary>
        public override void LoadBeforeAsync()
        {                
            _mapData = new MapData();
            gameLoaded = false;
            /*
            _mapData = MapData.Load();
            if (_mapData == null)
            {
                _mapData = new MapData();
                gameLoaded = false;
            }
            else
            {
                gameLoaded = true;
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UnLoad()
        {
            base.UnLoad();
            
            PoolManager.Instance.RemoveAll();
        }

        /// <summary>
        /// 미리 로딩해야 할 데이터 처리.
        /// </summary>
        public async override void Load()
        {
            if (gameLoaded == false)
            {
                var w = _mapData.Width;
                var h = _mapData.Height;
                var loopCount = w * h;
                var smoothCount = 5;
                var total = loopCount * 2 * smoothCount;
                var index = 0;

                var centerX = (int)(w * 0.5f);
                var centerZ = (int)(w * 0.5f);

                // 타일 생성.
                for (int x = 0; x < w; x++)
                {
                    for (int z = 0; z < h; z++)
                    {
                        var adress = x + z * w;
                        var tileData = new TileData();

                        if (x == 0 || x == w - 1 || z == 0 || z == h - 1)
                        {
                            tileData.type = TileType.Wall;
                        }
                        else if (x > centerX - 10 && x < centerX + 10 && z > centerZ - 10 && z < centerZ + 10)
                        {
                            tileData.type = TileType.Ground;
                        }
                        else
                        {
                            var waterPercent = 50;
                            var randColor = RandomNumber(0, 100);
                            if (randColor < waterPercent)
                                tileData.type = TileType.Water;
                            else
                                tileData.type = TileType.Ground;
                        }

                        Coordinate data = new Coordinate();
                        data.Tile = tileData;
                        _mapData.Data.Add(data);

                        index++;
                        Amount = (float)index / (float)total;
                    }
                }

                // 스무스 처리.
                for (var i = 0; i < smoothCount; i++)
                {
                    for (int x = 0; x < _mapData.Width; x++)
                    {
                        for (int z = 0; z < _mapData.Height; z++)
                        {
                            int adress = x + z * _mapData.Width;
                            if (_mapData.Data[adress].Tile.type == TileType.Wall)
                                continue;
                            int neighbourWallTiles = GetSurroundingWallCount(x, z);
                            if (neighbourWallTiles > 4)//map[x, y] = WALL; //주변 칸 중 벽이 4칸을 초과할 경우 현재 타일을 벽으로 바꿈
                            {
                                _mapData.Data[adress].Tile.type = TileType.Water;
                            }
                            else if (neighbourWallTiles < 4)//map[x, y] = ROAD; //주변 칸 중 벽이 4칸 미만일 경우 현재 타일을 빈 공간으로 바꿈
                            {
                                _mapData.Data[adress].Tile.type = TileType.Ground;
                            }

                            index++;
                            Amount = (float)index / (float)total;
                        }
                    }
                }

                // 나무 심기.
                for (var x = 0; x < w; x++)
                {
                    for (var z = 0; z < h; z++)
                    {
                        var adress = x + z * w;
                        var data = _mapData.Data[adress];
                        if (data != null)
                        {
                            if (data.Tile.type == TileType.Ground)
                            {
                                int rand = RandomNumber(1, 50);
                                if (rand < 10)
                                {
                                    if (data.Objects == null)
                                    {
                                        data.Objects = new List<ObjectData>();
                                        var obj = new ObjectData();
                                        obj.Id = RandomNumber(2, 9);
                                        data.Objects.Add(obj);
                                    }
                                }
                            }
                            else if (data.Tile.type == TileType.Water)
                            {
                                int rand = RandomNumber(1, 50);
                                if (rand < 20)
                                {
                                    if (data.Objects == null)
                                    {
                                        data.Objects = new List<ObjectData>();
                                        var obj = new ObjectData();
                                        obj.Id = 1;
                                        data.Objects.Add(obj);
                                    }
                                }
                            }
                        }

                        index++;
                        Amount = (float)index / (float)total;
                    }
                }

                // 벌 소환.
                for (var x = 0; x < w; x++)
                {
                    for (var z = 0; z < h; z++)
                    {
                        var adress = x + z * w;
                        var data = _mapData.Data[adress];
                        if (data != null)
                        {
                            if (data.Tile.type == TileType.Ground)
                            {
                                int rand = RandomNumber(1, 50);
                                if (rand < 20)
                                {
                                    if (data.Objects == null)
                                    {
                                        data.Objects = new List<ObjectData>();
                                        var obj = new ObjectData();
                                        obj.Id = 1000 + RandomNumber(0, 3);
                                        data.Objects.Add(obj);
                                    }
                                }
                            }
                            else if (data.Tile.type == TileType.Water)
                            {
                                int rand = RandomNumber(1, 50);
                                if (rand < 10)
                                {
                                    if (data.Objects == null)
                                    {
                                        data.Objects = new List<ObjectData>();
                                        var obj = new ObjectData();
                                        obj.Id = 2000;
                                        data.Objects.Add(obj);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Amount = 1f;
        }
        
        private int GetSurroundingWallCount(int gridX, int gridY)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            { //현재 좌표를 기준으로 주변 8칸 검사
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) 
                {
                    int adress = neighbourX + neighbourY * _mapData.Width;
                    if (neighbourX >= 0 && neighbourX < _mapData.Width && neighbourY >= 0 && neighbourY < _mapData.Height) { //맵 범위를 초과하지 않게 조건문으로 검사
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            //wallCount += _mapData[adress]; //벽은 1이고 빈 공간은 0이므로 벽일 경우 wallCount 증가
                            if (_mapData.Data[adress].Tile.type != TileType.Ground)
                            {
                                wallCount += 1;
                            }
                        }
                    }
                    else
                    {
                        wallCount++; //주변 타일이 맵 범위를 벗어날 경우 wallCount 증가
                    }
                }
            }
            
            return wallCount;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public void OnMove(Vector3 angle)
        {
            // 탑뷰 시점으로 변환.
            angle.z = angle.y;
            
            _pigeon.Move(angle);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnStop()
        {
  
        }

        private IEnumerator<float> UpdateTileObjects(int x, int y)
        {
            yield return MEC.Timing.WaitForOneFrame;
        }


        /// <summary>
        /// 
        /// </summary>
        public override void OnUpdate()
        {
            if (_pigeon == true)
            {
                var cameraPosition = _pigeon.transform.position + /*new Vector3(0, 1, 0) +*/
                                     _pigeon.transform.GetChild(0).forward * -_ditanceCamera;

                if (Vector3.Distance(MainCamera.transform.position, cameraPosition) > 0.1f)
                {                
                    MainCamera.transform.DOKill();
                    MainCamera.transform.DOMove(cameraPosition, 1f);
                }
                
                if((int)_pigeon.transform.position.x != _pigeon.x ||
                   (int)_pigeon.transform.position.z != _pigeon.z)
                {
                    _pigeon.x = (int)_pigeon.transform.position.x;
                    _pigeon.z = (int)_pigeon.transform.position.z;

                    // 새 좌표에 해당하는 타일로 업데이트.
                    handler.IsRunning = false;
                    handler = MEC.Timing.RunCoroutine(_map.UpdateTiles(_pigeon.x, _pigeon.z));
                   
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                OnMove(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                OnMove(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                OnMove(Vector3.up);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                OnMove(Vector3.down);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                OnStop();
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                OnStop();
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                OnStop();
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                OnStop();
            }
        }

        public override void OnTouchBean(Vector3 position)
        {
            _menu.joystick.TouchBegin(position);
        }

        public override void OnTouchEnd(Vector3 position)
        {
            _menu.joystick.TouchEnd(position);

            Ray ray = MainCamera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Card"))
                {
             
                }
            }
        }

        public override void OnTouchMove(Vector3 position)
        {
            _menu.joystick.TouchMove(position);
        }
    }
}