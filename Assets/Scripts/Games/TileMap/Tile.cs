using UnityEngine;

namespace Games.TileMap
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer _spriteRenderer;
        public int _x;
        public int _z;
    
        public bool Init(int x, int z)
        {
            _x = x;
            _z = z;
            
            var index = x + z;
            SetColor(index % 2 == 0 ? Color.gray : Color.white);
            
            transform.position = new Vector3(x, -.5f, z);
            return true;
        }

        public void SetColor(Color c)
        {
            _spriteRenderer.color = c;
        }
    }
}
