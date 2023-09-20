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
        private GameObject tree = null;
        
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
                
                string treeName = string.Empty;
                switch (data.Child)
                {
                    case 1:
                        treeName = "Bush";
                        break;
                    case 2:
                        treeName = "PineTree";
                        break;
                }
                
                if(treeName != string.Empty)
                {
                    if (tree == false)
                    {
                        var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>(treeName);
                        var obj = Object.Instantiate(prefab, transform);
                        obj.transform.localPosition = new Vector3(0, 1, 0);
                        tree = obj;                        
                    }

                }
                else
                {
                    tree?.gameObject.SetActive(false);
                }
            }
            else
            {
                tree?.gameObject.SetActive(false);
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
