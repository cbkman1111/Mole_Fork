using System;
using UnityEngine;

namespace TileMap
{
    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    public class WorldObject : MonoBehaviour
    {
        /// <summary>
        /// 타일의 좌표계.
        /// </summary>
        public int x;
        public int z;

        /// <summary>
        /// 타일 한개 내의 등분 좌표계.
        /// </summary>
        public Vector2 CordinateTileAdress { get; set; } = Vector2.zero;
        
        private Action<int, int> _onUpdatePosition;
        
        public bool Init(int posX, int posZ, Action<int, int> onUpdatePosition = null)
        {
            this.x = posX;
            this.z = posZ;
            
            _onUpdatePosition = onUpdatePosition;
            transform.position = new Vector3((float)posX, 0f, (float)posZ);
            return true;
        }
    }
}

