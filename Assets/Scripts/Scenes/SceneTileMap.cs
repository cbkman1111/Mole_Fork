using System;
using System.Collections.Generic;
using Common.Global;
using Common.Scene;
using Common.Utils.Pool;
using Creature;
using DG.Tweening;
using Games.TileMap;
using Games.TileMap.Datas;
using Spine.Unity;
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
        private UIMenuTileMap _menu = null;
        private Map _map = null;
        private Human _luke = null;
        private WorldObject _player = null;
        private float _ditanceCamera = 0;        
        RaycastHit[] _hits = new RaycastHit[10];

        public SkeletonAnimation skelDragon = null;
        public Buffalo _beefalo = null;

        private Vector3 lastMove = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            _ditanceCamera = 5;
            
            //MainCamera.transparencySortMode = UnityEngine.TransparencySortMode.Orthographic;
            //MainCamera.transparencySortAxis = new Vector3(0, 0, 1);

            _beefalo.Init(100, 101, Vector3.one);

            _menu = UIManager.Instance.OpenMenu<UIMenuTileMap>();
            if (_menu != null)
            {
                _menu.InitMenu(
                    move: (Vector3 angle) => {
                        lastMove = angle;
                        OnMove(lastMove);
                    },
                    stop: () => {
                        OnStop();
                    },
                    save:() =>
                    {
                        _mapData.X = _player.X;
                        _mapData.Z = _player.Z;
                        
                        _mapData.Save();
                    },
                    zoom: (float percent) =>
                    {
                        float amount = 6 * percent;
                        _ditanceCamera = 5 + amount;
                    },
                    nextHead:() => {
                        //_skelWoody.NextHeadSkin();
                        var skin = _beefalo.GetComponent<SkinMixAndMatch>();
                        if(skin != null)
                            skin.NextHeadSkin();
                    },
                    nextWeapone:() => {
                        //_skelWoody.NextWeaponeSkin();
                        var skin = _beefalo.GetComponent<SkinMixAndMatch>();
                        if(skin != null)
                            skin.NextWeaponeSkin();
                    },
                    seat:() => {
                        var player = _player;
                        _player = _beefalo;
                        _beefalo.Seat(player);
                    },
                    unSeat: () => {
                        _player = _beefalo.Unseat();
                    });
            }

            string pathPrefab = "Prefab";
            string pathCretures = $"{pathPrefab}/Creature";
            PoolManager.Instance.InitList(
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathPrefab}/TileGround"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathPrefab}/TileWater"),
                
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Bush"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/PineTree"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Sapling"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/SeaWeed"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Mushroom_empty"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Mushroom_green"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Mushroom_red"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Mushroom_seed"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Mushroom_sky"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Bee"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/Spider"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/FishMan"),
                ResourcesManager.Instance.LoadInBuild<Transform>($"{pathCretures}/FishA"));
            
            var startX = _mapData.X;
            var startZ = _mapData.Z;
            var width = _mapData.Width;
            var height = _mapData.Height;
            
            // 맵 생성.
            const int displayW = 10;
            const int displayUpSide = 10;
            const int displayDownSide = 10;

            var prefabMap = ResourcesManager.Instance.LoadInBuild<Map>($"{pathPrefab}/Map");
            _map = GameObject.Instantiate<Map>(prefabMap);
            _map.transform.position = Vector3.zero;
            _map.Init(_mapData, startX, startZ, displayW, displayUpSide, displayDownSide);
            
            // 캐릭터 생성.
            var prefab = ResourcesManager.Instance.LoadInBuild<Human>($"{pathPrefab}/Player");
            _luke = GameObject.Instantiate<Human>(prefab);
            _luke.name = "luke";
            _luke.Init(startX, startZ, Vector3.one);

            _player = _luke;

            // 카메라 위치 초기화.
            MainCamera.transform.position = Vector3.zero;
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
        /// 
        /// </summary>
        public override void UnLoad()
        {
            base.UnLoad();
            
            PoolManager.Instance.RemoveAll();
            
            MainCamera.transform.DOKill();
            _player.transform.DOKill();
        }

        /// <summary>
        /// 미리 로딩해야 할 데이터 처리.
        /// </summary>
        public async override void Load(Action<float> update)
        {
            _mapData = new MapData();

            var w = _mapData.Width;
            var h = _mapData.Height;
            //var loopCount = w * h;
            var smoothCount = 5;
            //var total = loopCount * 2 * smoothCount;
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
                    //Amount = (float)index / (float)total;
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
                        //Amount = (float)index / (float)total;
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
                    //Amount = (float)index / (float)total;
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
     

            update(1f);
            //Amount = 1f;
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
        
        public void OnMove(Vector3 angle)
        {
            // 탑뷰 시점으로 변환.
            angle.z = angle.y;

            IMove moveAble = _player as IMove;
            if (moveAble != null)
            {
                moveAble.Move(angle);
            }
        }

        public void OnDash(Vector3 angle)
        {
            // 탑뷰 시점으로 변환.
            angle.z = angle.y;
            IMove moveAble = _player as IMove;
            if (moveAble != null)
            {
                moveAble.Dash(angle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnStop()
        {
            IMove moveAble = _player as IMove;
            if (moveAble != null)
            {
                moveAble.Stop();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void OnUpdate()
        {
            if (_player != null)
            {
                var cameraPosition = _player.transform.position + _player.transform.GetChild(0).forward * -_ditanceCamera;
                
                if (Vector3.Distance(MainCamera.transform.position, cameraPosition) > 0.1f)
                {
                    if (MainCamera.transform.position == Vector3.zero)
                    {
                        MainCamera.transform.position = cameraPosition;
                    }
                    else
                    {
                        MainCamera.transform.DOKill();
                        MainCamera.transform.DOMove(cameraPosition, 1f);
                    }
                }
                
                if((int)_player.transform.position.x != _player.X ||
                   (int)_player.transform.position.z != _player.Z)
                {
                    _player.X = (int)_player.transform.position.x;
                    _player.Z = (int)_player.transform.position.z * 100;
                }
            }


            if (Input.GetKey(KeyCode.A))
            {
                lastMove = Vector3.left;
                OnMove(lastMove);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                lastMove = Vector3.right;
                OnMove(lastMove);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                lastMove = Vector3.up;
                OnMove(lastMove);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                lastMove = Vector3.down;
                OnMove(lastMove);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                OnDash(lastMove);
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
            _menu.SetObjectInfo(string.Empty);
            
            Ray ray = MainCamera.ScreenPointToRay(position);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 2f);
            //, LayerMask.NameToLayer("WorldObject")
            var count = Physics.RaycastNonAlloc(ray.origin, ray.direction, _hits,100f);
            if (count > 0 && _hits.Length > 0)
            {
                int nearIndex = 0;
                float nearDistance = 9999f;
                for (int i = 0; i < count; i++)
                {
                    var distance = Vector3.Distance(_hits[i].transform.position, position);
                    if (distance < nearDistance)
                    {
                        nearDistance = distance;
                        nearIndex = i;
                    }
                }

                //var nearList = _hits.OrderBy(h => Vector3.Distance(h.transform.position, position)).ToList();
                var obj = _hits[nearIndex].collider.gameObject;
                var worldObject = obj.GetComponent<WorldObject>();
                if (worldObject != null)
                {
                    worldObject.ChangeState(ObjectState.Click);
                    _menu.SetObjectInfo(obj.name);
                }
            }

            /*
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (layer == LayerMask.NameToLayer("WorldObject"))
                {
                    var obj = hit.collider.gameObject;
                    var worldObject = obj.GetComponent<WorldObject>();
                    worldObject.SetState(ObjectState.Click);
                    _menu.SetObjectInfo(obj.name);
                }

                Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
                Debug.Log(hit.point);
            }
            */
            
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

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {
            _menu.joystick.TouchMove(position);
        }
    }
}