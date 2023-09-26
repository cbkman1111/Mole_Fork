using System.Collections.Generic;
using System.Linq;
using Common.Utils.Pool;
using Games.TileMap.Datas;
using TileMap;
using UnityEngine;
using MapData = Games.TileMap.Datas.MapData;

namespace Games.TileMap
{
    public class Map : MonoBehaviour
    {
        private MapData _mapData;
       
        private int _diplayWidth;
        private int _diplayUpSide;
        private int _diplayDownSide;
        
        private List<Tile> tiles = new List<Tile>();
        private List<WorldObject> objects = new List<WorldObject>();
        
        public bool Init(MapData data, int startX, int startZ, int diplayW, int displayUpSide, int displayDownSide)
        {
            _mapData = data;

            _diplayWidth = diplayW;
            _diplayUpSide = displayUpSide;
            _diplayDownSide = displayDownSide;
           
            for (var x = startX - _diplayWidth; x <= startX + _diplayWidth; x++)
            {
                for (var z = startZ - _diplayDownSide; z <= startZ + _diplayUpSide; z++)
                {
                    var adress = x + z * _mapData.Width;
                    if (adress < 0 || adress >= _mapData.Data.Count)
                        continue;

                    var tileData = _mapData.Data[adress].Tile;
                    var objList = _mapData.Data[adress].Objects;

                    // 타일 리스트.
                    if (tileData != null)
                    {
                        var tile = GetTile(tileData);
                        if (tile == true)
                        {
                            tileData.obj = tile;
                            tile.Init(tileData, x, z);
                            tiles.Add(tile);
                        }
                    }

                    // 오브젝트 리스트.
                    if (objList != null)
                    {
                        var obj = GetWorldObject(objList[0]);
                        if (obj == true)
                        {
                            objList[0].obj = obj;
                            obj.transform.position = new Vector3(x, 0.5f, z);
                            objects.Add(obj);
                        }
                    }

                    if (x < 0 || x > _mapData.Width || z < 0 || z > _mapData.Height)
                    {

                    }
                    else
                    {
                       
                    }
                }
            }
           
            return true;
        }

        private Tile GetTile(TileData data)
        {                            
            Transform trans = null;
            if(data.type == TileType.Ground)
                trans =  PoolManager.Instance.GetObject("TileGround");
            else
                trans =  PoolManager.Instance.GetObject("TileWater");

            if (trans == null)
                return null;

            var tile = trans.GetComponent<Tile>();
            return tile;
        }

        private WorldObject GetWorldObject(ObjectData data)
        {
            

            string[] names =
            {
                "SeaWeed",
                
                "Bush",
                "PineTree",
                "Sapling",
                "Mushroom_empty",
                "Mushroom_green",
                "Mushroom_red",
                "Mushroom_sky",
                "Mushroom_seed",
            };

            string path = "";
            if (data.Id == 1000)
            {
                path = "Bee";
            }
            else if (data.Id == 1001)
            {
                path = "Spider";
            }
            else if (data.Id == 1002)
            {
                path = "FishMan";
            }
            else if (data.Id == 2000)
            {
                path = "FishA";
            }
            else
            {
                path = names[data.Id - 1];
            }

            Transform trans = PoolManager.Instance.GetObject(path);
            if (trans == true)
            {           
                var obj = trans.GetComponent<WorldObject>();
                return obj;
            }

            return null;
        }

        //private IEnumerator<float> UpdateTileObjects(int x, int y)
        /// <summary>
        /// 타일 정보 갱신.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startZ"></param>
        public IEnumerator<float> UpdateTiles(int startX, int startZ)
        {
            var listW = tiles.OrderBy(t => t._x);
            var ListH = tiles.OrderBy(t => t._z);
            var left = listW.First()._x;
            var right = listW.Last()._x;
            var down = ListH.First()._z;
            var up = ListH.Last()._z;

            if (left > startX - _diplayWidth)
                left = startX - _diplayWidth;
            if (right < startX + _diplayWidth)
                right = startX + _diplayWidth;

            if (down > startZ - _diplayDownSide)
                down = startZ - _diplayDownSide;
            if (up < startZ + _diplayUpSide)
                up = startZ + _diplayUpSide;

            yield return MEC.Timing.WaitForOneFrame;

            // 나간 리스트 처리.
            for (var x = left; x <= right; x++)
            {
                for (var z = down; z <= up; z++)
                {
                    int adress = x + z * _mapData.Width;
                    if (adress < 0 || _mapData.Data.Count <= adress)
                        continue;

                    var tileData = _mapData.Data[adress].Tile;
                    var objDataList = _mapData.Data[adress].Objects;
                    // 바깥쪽.
                    if (x < startX - _diplayWidth ||
                        x > startX + _diplayWidth ||
                        z < startZ - _diplayDownSide ||
                        z > startZ + _diplayUpSide)
                    {
                        if (tileData.obj != null)
                        {
                            PoolManager.Instance.ReturnObject(tileData.obj.transform);
                            tiles.Remove(tileData.obj);
                            tileData.obj = null;
                        }

                        if (objDataList != null)
                        {
                            foreach (var objData in objDataList)
                            {
                                if (objData != null && objData.obj != null)
                                {
                                    PoolManager.Instance.ReturnObject(objData.obj.transform);
                                    objects.Remove(objData.obj);
                                    objData.obj = null;
                                }
                            }
                        }
                    }
                }
            }

            yield return MEC.Timing.WaitForOneFrame;

            left = startX - _diplayWidth;
            right = startX + _diplayWidth;
            down = startZ - _diplayDownSide;
            up = startZ + _diplayUpSide;

            for (var x = left; x <= right; x++)
            {
                for (var z = down; z <= up; z++)
                {
                    int adress = x + z * _mapData.Width;
                    if (adress < 0 || _mapData.Data.Count <= adress)
                        continue;

                    var tileData = _mapData.Data[adress].Tile;
                    var objDataList = _mapData.Data[adress].Objects;
                    // 안쪽.
                    if (x >= startX - _diplayWidth ||
                        x <= startX + _diplayWidth ||
                        z >= startZ - _diplayDownSide ||
                        z <= startZ + _diplayUpSide)
                    {
                        if (tileData.obj == null)
                        {
                            var tile = GetTile(tileData);
                            if (tile == false)
                                continue;

                            if (x < 0 || x > _mapData.Width || z < 0 || z > _mapData.Height)
                            {
                                tile.Init(null, x, z);
                            }
                            else
                            {
                                tile.Init(tileData, x, z);
                            }

                            tileData.obj = tile;
                            tiles.Add(tile);
                        }

                        if (objDataList != null)
                        {
                            foreach (var objData in objDataList)
                            {
                                if (objData.obj == null)
                                {
                                    WorldObject obj = GetWorldObject(objData);
                                    if (obj == true)
                                    {
                                        obj.transform.position = new Vector3(x, 0.5f, z);
                                        objData.obj = obj;
                                        objects.Add(obj);
                                    }
                                }
                            }
                        };
                    }
                }
            }
        }
    }
}
