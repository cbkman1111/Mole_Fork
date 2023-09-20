using System.Collections.Generic;
using Common.Global;
using Common.Scene;
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
        
        private MapData _mapData = new MapData();
        
        private UIMenuTileMap _menu = null;
        private Map _map = null;
        private Pige _pigeon = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
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
                    () =>
                    {
                        _mapData.X = _pigeon.x;
                        _mapData.Z = _pigeon.z;
                        
                        _mapData.Save();
                    });
            }

            int startX = _mapData.X;
            int startZ = _mapData.Z;
            // 맵 생성.
            var prefabMap = ResourcesManager.Instance.LoadInBuild<Map>("Map");
            _map = Object.Instantiate<Map>(prefabMap);
            _map.transform.position = Vector3.zero;
            var displayW = 13;
            var displayUpSide = 6;
            var displayDownSide = 4;
            _map.Init(_mapData, startX, startZ, _mapData.Width, _mapData.Height, displayW, displayUpSide, displayDownSide);
            
            // 캐릭터 생성.
            var prefab = ResourcesManager.Instance.LoadInBuild<Pige>("PigeonTemp");
            _pigeon = Object.Instantiate<Pige>(prefab);
            _pigeon.Init(startX, startZ);
            
            // 카메라 위치 초기화.
            MainCamera.transform.position = _pigeon.transform.position + _pigeon.transform.GetChild(0).forward * -10;
            return true;
        }

        public static int RandomNumber(int min, int max)
        {
            lock (synlock)
            {
                return random.Next(min, max);
            }
        }
        
        /// <summary>
        /// 미리 로딩해야 할 데이터 처리.
        /// </summary>
        public async override void Load()
        {
            /*
            _mapData = MapData.Load();
            if (_mapData != null)
            {
                Amount = 1.0f;
                return;
            }
            */
            int w = _mapData.Width;
            int h = _mapData.Height;
            int loopCount = w * h;
            int smoothCount = 5;
            int total = loopCount * 2 * smoothCount;

            int index = 0;
            for (int x = 0; x < w; x++)
            {
                for (int z = 0; z < h; z++)
                {
                    var adress = x + z * w;
                    var info = new TileData();
                    info.Color = Color.gray;
                    
                    if (x == 0 || x == w - 1 || z == 0 || z == h - 1)
                    {
                        info.Color = Color.black;
                    }
                    else
                    {
                        var waterPercent = 50;
                        var randColor = RandomNumber(0, 100);
                        info.Color = (randColor < waterPercent) ? Color.blue : Color.gray; //비율에 따라 벽 혹은 빈 공간 생성
                    }

                    _mapData.TileData.TryAdd(adress, info);
                    
                    index++;
                    Amount = (float)index / (float)total;
                }
            }

            // 
            for (var i = 0; i < smoothCount; i++)
            {
                for (int x = 0; x < _mapData.Width; x++) 
                {
                    for (int y = 0; y < _mapData.Height; y++) 
                    {
                        int adress = x + y * _mapData.Width;
                        if (_mapData.TileData[adress].Color == Color.black)
                            continue;
                    
                        int neighbourWallTiles = GetSurroundingWallCount(x, y);
                        if (neighbourWallTiles > 4)//map[x, y] = WALL; //주변 칸 중 벽이 4칸을 초과할 경우 현재 타일을 벽으로 바꿈
                        {
                            _mapData.TileData[adress].Color = Color.blue;
                        }
                        else if (neighbourWallTiles < 4)//map[x, y] = ROAD; //주변 칸 중 벽이 4칸 미만일 경우 현재 타일을 빈 공간으로 바꿈
                        {
                            _mapData.TileData[adress].Color = Color.gray;
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
                    if (_mapData.TileData.TryGetValue(adress, out var tile) == true)
                    {
                        if (tile.Color == Color.gray)
                        {
                            var randTree = RandomNumber(0, 10);
                            tile.Child = randTree;
                        }
                    }
                    
                    index++;
                    Amount = (float)index / (float)total;
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
                            if (_mapData.TileData[adress].Color != Color.gray)
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

        /// <summary>
        /// 
        /// </summary>
        public override void OnUpdate()
        {
            if (_pigeon == true)
            {
                var cameraPosition = _pigeon.transform.position + _pigeon.transform.GetChild(0).forward * -10;
                //MainCamera.transform.position = cameraPosition;
                MainCamera.transform.DOMove(cameraPosition, 1f);
                
                if((int)_pigeon.transform.position.x != _pigeon.x ||
                   (int)_pigeon.transform.position.z != _pigeon.z)
                {
                    _pigeon.x = (int)_pigeon.transform.position.x;
                    _pigeon.z = (int)_pigeon.transform.position.z;
                    
                    // 새 좌표에 해당하는 타일로 업데이트.
                    _map.UpdateTiles(_pigeon.x, _pigeon.z);
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