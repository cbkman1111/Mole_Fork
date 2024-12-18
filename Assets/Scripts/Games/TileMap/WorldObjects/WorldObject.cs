using DG.Tweening;
using Scenes.EllersAlgorithm;
using UnityEngine;

namespace Creature
{
    public class Stat
    {
        public int hp;
        public int attack;
        public int defense;
        public int speed;
    }

    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    public partial class WorldObject : StateMachine
    {
        /// <summary>
        /// 타일의 좌표계.
        /// </summary>
        public int X { get; set; }
        public int Z { get; set; }
        protected Stat Stat { get; set; } = new Stat();

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

        public override void OnEnterState(ObjectState state) { }
        public override void OnExitState(ObjectState state) { }
    }
}