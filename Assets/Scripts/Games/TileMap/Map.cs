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
                    if (x < 0 || x > _mapData.Width || z < 0 || z > _mapData.Height)
                    {

                    }
                    else
                    {
                        var adress = x + z * _mapData.Width;
                        var tileData = _mapData.Data[adress].Tile;
                        var objList = _mapData.Data[adress].Objects;
                        
                        // 타일 리스트.
                        if(tileData != null)
                        {
                            var tile = GetTile(tileData);
                            if (tile == true)
                            {
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
                                obj.transform.position = new Vector3(x ,0.5f, z);
                                objects.Add(obj);
                            }
                        }
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
            
            Transform trans = PoolManager.Instance.GetObject(names[data.Id - 1]);
            if (trans == true)
            {           
                var obj = trans.GetComponent<WorldObject>();
                return obj;
            }

            return null;
        }

        /// <summary>
        /// 타일 정보 갱신.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startZ"></param>
        public void UpdateTiles(int startX, int startZ)
        {
            var removeList = tiles.Where(t =>
                    t._x < startX - _diplayWidth ||
                    t._x > startX + _diplayWidth || 
                    t._z < startZ - _diplayDownSide ||
                    t._z > startZ + _diplayUpSide)
                .ToList();

            var removeObjectList = objects.Where(t =>
                t.x < startX - _diplayWidth ||
                t.x > startX + _diplayWidth ||
                t.z < startZ - _diplayDownSide ||
                t.z > startZ + _diplayUpSide).ToList();
                
            foreach (var tile in removeList)
            { 
                PoolManager.Instance.ReturnObject(tile.transform);
                tiles.Remove(tile);
            }
            
            foreach (var obj in removeObjectList)
            { 
                PoolManager.Instance.ReturnObject(obj.transform);
                objects.Remove(obj);
            }
            
            for (var x = startX - _diplayWidth; x < startX + _diplayWidth; x++)
            {
                for (var z = startZ - _diplayDownSide ; z < startZ + _diplayUpSide; z++)
                {
                    if(x < 0 || x > _mapData.Width)
                        continue;
                    
                    if(z < 0 || z > _mapData.Height)
                        continue;
                    
                    int adress = x + z * _mapData.Width;
                    var tileData = _mapData.Data[adress].Tile;
                    
                    var iter = tiles.Where(t => t._x == x && t._z == z);
                    if (iter.Count() == 0 )
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
                        
                        tiles.Add(tile);
                    }
                    
                    var objList = _mapData.Data[adress].Objects;
                    if (objList != null)
                    {
                        WorldObject obj = GetWorldObject(objList[0]);
                        if (obj == true)
                        {                        
                            obj.transform.position = new Vector3(x ,0.5f, z);
                            objects.Add(obj);
                        }
                    }
                }
            }
        }
    }
}
