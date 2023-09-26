using Common.Global;
using Games.TileMap.Datas;
using Scenes;
using UnityEngine;

namespace Games.TileMap
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer _spriteRenderer;
        public int _x;
        public int _z;
        
        public bool Init(TileData data, int x, int z)
        {
            _x = x;
            _z = z;
            
            if (data != null)
            {
                switch (data.type)
                {
                    case TileType.Ground:
                        SetColor(Color.gray);
                        break;
                    case TileType.Water:
                        SetColor(new Color(0.2117647f, 0.5686183f, 0.5450981f, 0.5f));
                        break;
                    case TileType.Wall:
                        SetColor(Color.black);
                        break;
                }
            }
            else
            {
                SetColor(Color.black);
            }

            transform.position = new Vector3(x, -.5f, z);
            return true;
        }

        public void SetColor(Color c)
        {
            if(_spriteRenderer != null)
                _spriteRenderer.color = c;
        }
    }
}
