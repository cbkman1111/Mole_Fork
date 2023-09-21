using System.Collections.Generic;
using System.Linq;
using Common.Utils.Pool;
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
                    var obj =  PoolManager.Instance.GetObject("Tile");
                    var tile = obj.GetComponent<Tile>();
                    if (tile == false)
                        continue;

                    if (x < 0 || x > _mapData.Width || z < 0 || z > _mapData.Height)
                    {
                        tile.Init(null, x, z);
                    }
                    else
                    {
                        var adress = x + z * _mapData.Width;
                        var tileData = _mapData.Data[adress].Tile;
                        if(tileData != null)
                        {
                            tile.Init(tileData, x, z);
                        }
                        else
                        {
                            tile.Init(null, x, z);
                        }

                        var objList = _mapData.Data[adress].Objects;
                        if (objList != null)
                        {
                            Transform trans = null;
                            var objData = objList[0];
                            switch (objData.Id)
                            {
                                case 1:
                                    trans = PoolManager.Instance.GetObject("Bush");
                                    var bush = trans.GetComponent<Bush>();
                                    if (bush != null)
                                    {                        
                                        bush.transform.position = new Vector3(x ,0.5f, z);
                                        objects.Add(bush);
                                    }
                                    break;
                                case 2:
                                    trans = PoolManager.Instance.GetObject("PineTree");
                                    var pineTree = trans.GetComponent<PineTree>();
                                    if (pineTree != null)
                                    {
                                        pineTree.transform.position = new Vector3(x ,0.5f, z);
                                        objects.Add(pineTree);
                                    }
                                    break;
                            }
                        }
                    }
                   
                    tiles.Add(tile);
                }
            }
           
            return true;
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
                    int adress = x + z * _mapData.Width;
                    var iter = tiles.Where(t => t._x == x && t._z == z);
                    if (iter.Count() == 0)
                    {
                        var obj = PoolManager.Instance.GetObject("Tile");
                        var tile = obj.GetComponent<Tile>();
                        if (tile == false)
                        {
                            continue;
                        }
                        
                        if (x < 0 || x > _mapData.Width || z < 0 || z > _mapData.Height)
                        {
                            tile.Init(null, x, z);
                        }
                        else
                        {
                            
                            var info = _mapData.Data[adress].Tile;
                            tile.Init(info, x, z);
                        }
                        
                        tiles.Add(tile);
                    }
                    
                    var objList = _mapData.Data[adress].Objects;
                    if (objList != null)
                    {
                        Transform trans = null;
                        var objData = objList[0];
                        switch (objData.Id)
                        {
                            case 1:
                                trans = PoolManager.Instance.GetObject("Bush");
                                var bush = trans.GetComponent<Bush>();
                                if (bush != null)
                                {                        
                                    bush.transform.position = new Vector3(x ,0.5f, z);
                                    objects.Add(bush);
                                }
                                break;
                            case 2:
                                trans = PoolManager.Instance.GetObject("PineTree");
                                var pineTree = trans.GetComponent<PineTree>();
                                if (pineTree != null)
                                {
                                    pineTree.transform.position = new Vector3(x ,0.5f, z);
                                    objects.Add(pineTree);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
