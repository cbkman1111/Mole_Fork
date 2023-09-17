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
        public Vector2 CordinateTile { get; set; } = Vector2.zero;

        /// <summary>
        /// 타일 한개 내의 등분 좌표계.
        /// </summary>
        public Vector2 CordinateTileAdress { get; set; } = Vector2.zero;
    }
}


