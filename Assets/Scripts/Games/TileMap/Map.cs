using System.Collections.Generic;
using System.Linq;
using Common.Utils.Pool;
using Games.Sea;
using Scenes;
using UnityEngine;

namespace Games.TileMap
{
    public class Map : MonoBehaviour
    {
        private Dictionary<int, SceneTileMap.MapTile> mapData;
        
        private int _width;
        private int _height;
        private int _diplayWidth;
        private int _diplayHeight;
        
        [SerializeField] public Tile _tilePrefab = null;
        private Pool<Transform> _pool;

        private List<Tile> tiles = new List<Tile>();
        
        public bool Init(Dictionary<int, SceneTileMap.MapTile> data, int startX, int startZ, int width, int height, int diplayW, int displayH)
        {
            mapData = data;
            _diplayWidth = diplayW;
            _diplayHeight = displayH;
            _width = width;
            _height = height;

            _pool = Pool<Transform>.Create(_tilePrefab.transform, transform, 100);
            
            for (var x = startX - _diplayWidth; x <= startX + _diplayWidth; x++)
            {
                for (var z = startZ - _diplayHeight; z <= startZ + _diplayHeight; z++)
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
                        mapData.TryGetValue(adress, out var info);
                        tile.Init(info, x, z);
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
                for (var z = startZ - _diplayHeight ; z < startZ + _diplayHeight; z++)
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
                            mapData.TryGetValue(adress, out var info);
                            tile.Init(info, x, z);
                        }
                        
                        tiles.Add(tile);
                    }
                }
            }
        }
    }
}
