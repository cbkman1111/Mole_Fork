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
                SetColor(data.Color);
                if (data.Color == Color.blue)
                    _spriteRenderer.transform.localPosition = new Vector3(0, 0, 0);
                else
                    _spriteRenderer.transform.localPosition = new Vector3(0, 0.5f, 0);
            }
            else
            {
                //tree?.gameObject.SetActive(false);
                SetColor(Color.black);
            }

            transform.position = new Vector3(x, -.5f, z);
            return true;
        }

        public void SetColor(Color c)
        {
            _spriteRenderer.color = c;
        }
    }
}
