using Common.Global;
using GoogleMobileAds.Api;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    //public partial class WorldObject : StateMachine
    public partial class WorldObject : MonoBehaviour //StateMachine
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

        [SerializeField] protected NavMeshAgent _navMeshAgent;
        //[SerializeField] protected Behaviour _agnet = null;
        [SerializeField] protected BehaviorGraphAgent _agent = null;

        /// <summary>
        /// 타일의 좌표계.
        /// </summary>
        public int X { get; set; }
        public int Z { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// 스탯.
        /// </summary>
        public Stat Stat { get; set; } = new Stat();

        public static WorldObject Create(string path, Transform parent, int x, int z)
        {
            var go = ResourcesManager.Instance.LoadBundle($"{path}");
            if (go == null)
                return default;

            var obj = Instantiate<WorldObject>(go.GetComponent<WorldObject>(), parent);
            //var obj = go.GetComponent<WorldObject>();
            if (obj != null && obj.Init(x, z) == true)
            {
                return obj;
            }

            Destroy(go);
            return default;
        }

        /// <summary>
        /// 객체 초기화.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posZ"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public bool Init(int x, int z)
        {
            X = x;
            Z = z;
            Y = 0;

            transform.position = new Vector3(X, Y, Z);
            transform.localScale = Vector3.one;
            
            InitSpine();
            InitStat();
            _agent.BlackboardReference.SetVariableValue("Self", gameObject);
            //ChangeState(ObjectState.Idle);
            return true;
        }

        public void InitStat()
        {
            Stat.Health = 100;   
        }

        /// <summary>
        /// 방향 얻기.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
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

            //Common.Utils.GiantDebug.Log($"GetDirect: {angle} -> {dir}");
            return dir;
        }

        //public override void OnStateEnter(ObjectState state) { }
        //public override void OnStateExit(ObjectState state) { }
    }
}