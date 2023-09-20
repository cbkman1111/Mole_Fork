using System.Collections.Generic;
using System.Linq;
using Common.Utils.Pool;
using Scenes;
using UnityEngine;
using MapData = Games.TileMap.Datas.MapData;

namespace Games.TileMap
{
    public class Map : MonoBehaviour
    {
        private MapData mapData;
        private int _width;
        private int _height;
        
        private int _diplayWidth;
        private int _diplayUpSide;
        private int _diplayDownSide;
        
        [SerializeField] public Tile _tilePrefab = null;
        private Pool<Transform> _pool;

        private List<Tile> tiles = new List<Tile>();
        
        public bool Init(MapData data, int startX, int startZ, int width, int height, int diplayW, int displayUpSide, int displayDownSide)
        {
            mapData = data;
            _diplayWidth = diplayW;
            _diplayUpSide = displayUpSide;
            _diplayDownSide = displayDownSide;
            _width = width;
            _height = height;

            _pool = Pool<Transform>.Create(_tilePrefab.transform, transform, 100);
            
            for (var x = startX - _diplayWidth; x <= startX + _diplayWidth; x++)
            {
                for (var z = startZ - _diplayDownSide; z <= startZ + _diplayUpSide; z++)
                {
                    var obj = _pool.GetObject();
                    var tile = obj.GetComponent<Tile>();
                    if (tile == false)
                        continue;

                    if (x < 0 || x > _width || z < 0 || z > _height)
                    {
                        tile.Init(null, x, z);
                    }
                    else
                    {
                        int adress = x + z * _width;
                        if(mapData.TileData.TryGetValue(adress, out var info) == true)
                        {
                            tile.Init(info, x, z);    
                        }
                        else
                        {
                            tile.Init(null, x, z);
                        }
                    }
                        
                    tiles.Add(tile);
                }
            }

            return true;
        }
        
        public void UpdateTiles(int startX, int startZ)
        {
            var removeList = tiles.Where(t =>
                    t._x < startX - _diplayWidth ||
                    t._x > startX + _diplayWidth || 
                    t._z < startZ - _diplayWidth ||
                    t._z > startZ + _diplayWidth)
                .ToList();
            
            foreach (var tile in removeList)
            {
                _pool.ReturnObject(tile.transform);
                tiles.Remove(tile);
            }
            
            for (var x = startX - _diplayWidth; x < startX + _diplayWidth; x++)
            {
                for (var z = startZ - _diplayDownSide ; z < startZ + _diplayUpSide; z++)
                {
                    var iter = tiles.Where(t => t._x == x && t._z == z);
                    if (iter.Count() == 0)
                    {
                        var obj = _pool.GetObject();
                        var tile = obj.GetComponent<Tile>();
                        if (tile == false)
                        {
                            continue;
                        }
                        
                        if (x < 0 || x > _width || z < 0 || z > _height)
                        {
                            tile.Init(null, x, z);
                        }
                        else
                        {
                            int adress = x + z * _width;
                            mapData.TileData.TryGetValue(adress, out var info);
                            tile.Init(info, x, z);
                        }
                        
                        tiles.Add(tile);
                    }
                }
            }
        }
    }
}
