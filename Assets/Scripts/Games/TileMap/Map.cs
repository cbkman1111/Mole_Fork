using System.Collections.Generic;
using System.Linq;
using Common.Utils.Pool;
using Games.Sea;
using UnityEngine;

namespace Games.TileMap
{
    public class Map : MonoBehaviour
    {
        private int _width;
        private int _height;
        
        [SerializeField] public Tile _tilePrefab = null;
        private Pool<Transform> _pool;

        private List<Tile> tiles = new List<Tile>();
        public bool Init(int startX, int startZ, int w, int h)
        {
            _width = w;
            _height = h;

            _pool = Pool<Transform>.Create(_tilePrefab.transform, transform, 100);
            
            for (var x = startX - _width; x <= startX + _width; x++)
            {
                for (var z = startZ - _height; z <= startZ + _height; z++)
                {
                    var obj = _pool.GetObject();
                    var tile = obj.GetComponent<Tile>();
                    if (tile == false)
                    {
                        continue;
                    }
                    
                    tile.Init(x, z);
                    tiles.Add(tile);
                }
            }

            return true;
        }

        public void AddTiles(int x, int y)
        {
            
        }
        
        public void UpdateTiles(int startX, int startZ)
        {
            var removeList = tiles.Where(t =>
                    t._x < startX - _width ||
                    t._x > startX + _width || 
                    t._z < startZ - _height ||
                    t._z > startZ + _height)
                .ToList();
            
            foreach (var tile in removeList)
            {
                _pool.ReturnObject(tile.transform);
                tiles.Remove(tile);
            }
            
            for (var x = startX - _width; x < startX + _width; x++)
            {
                for (var z = startZ - _height; z < startZ + _height; z++)
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
                    
                        tile.Init(x, z);
                        tiles.Add(tile);
                    }
                }
            }
        }
    }
}
