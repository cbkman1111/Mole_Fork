using DG.Tweening;
using Scenes.EllersAlgorithm;
using UnityEngine;

namespace Creature
{
    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    public partial class WorldObject : StateMachine
    {
        [System.Flags]
        public enum Direct
        {
            None = 0,
            Up = 1 << 0, // 0001
            Down = 1 << 1, // 0010
            Left = 1 << 2, // 0100
            Right = 1 << 3  // 1000
        }

        [HideInInspector]
        public Direct Direction { get; set; } = Direct.Down;

        /// <summary>
        /// 타일의 좌표계.
        /// </summary>
        public int X { get; set; }
        public int Z { get; set; }
        public Stat Stat { get; set; } = new Stat();

        /// <summary>
        /// 객체 초기화.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posZ"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public bool Init(int x, int z, Vector3 scale)
        {
            X = x;
            Z = z;
            transform.position = new Vector3(X, 0, Z);
            transform.localScale = scale;

            InitSpine();
            ChangeState(ObjectState.Idle);

            
            return true;
        }

        protected Direct GetDirect(Vector3 angle)
        {
            Direct dir = Direct.None;

            if (angle.z > 0f)
                dir = Direct.Up;
            else if (angle.z < 0f)
                dir = Direct.Down;

            if (angle.x < 0f)
                dir |= Direct.Left;
            else
                dir |= Direct.Right;

            Common.Utils.GiantDebug.Log($"GetDirect: {angle} -> {dir}");
            return dir;
        }

        public override void OnStateEnter(ObjectState state) { }
        public override void OnStateExit(ObjectState state) { }
    }
}