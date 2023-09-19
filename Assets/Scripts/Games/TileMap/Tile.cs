using Scenes;
using UnityEngine;

namespace Games.TileMap
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer _spriteRenderer;
        public int _x;
        public int _z;
    
        public bool Init(SceneTileMap.MapTile data, int x, int z)
        {
            _x = x;
            _z = z;
            
            if(data== null)
                SetColor(Color.black);
            else
                SetColor(data.Color);
            transform.position = new Vector3(x, -.5f, z);
            return true;
        }

        public void SetColor(Color c)
        {
            _spriteRenderer.color = c;
        }
    }
}
