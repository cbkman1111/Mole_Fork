using System;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityCapsuleCollider;
using Common.Global;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    //public partial class WorldObject : StateMachine
    public partial class WorldObject : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI _Message = null;
        [SerializeField] protected NavMeshAgent _navMeshAgent;
        [SerializeField] protected BehaviorGraphAgent _agent = null;
        public BlackboardReference BlackboardReference => _agent.BlackboardReference;
        protected CretureStateMachine stateMachine = new CretureStateMachine();

        [System.Flags]
        public enum Direct
        {
            None = 0,
            Up = 1 << 0, // 0001
            Down = 1 << 1, // 0010
            Left = 1 << 2, // 0100
            Right = 1 << 3  // 1000
        }

        /// <summary>
        /// 타일의 좌표계.
        /// </summary>
        public int X { get; set; }
        public int Z { get; set; }
        public int Y { get; set; }
        [HideInInspector] public Direct Direction { get; set; } = Direct.Down;

        /// <summary>
        /// 스탯.
        /// </summary>
        public Stat Stat { get; set; } = new Stat();

        public static WorldObject Create(string path, Transform parent, int x, int z)
        {
            var go = ResourcesManager.Instance.LoadBundle($"{path}");
            if (go == null)
                return null;

            var component = go.GetComponent<WorldObject>();
            if (component == null)
                return null;

            var obj = Instantiate(component, parent);
            if (obj != null && obj.Init(x, z) == true)
            {
                return obj;
            }

            Destroy(go);
            return null;
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

            if (_navMeshAgent != null)
            {
                _navMeshAgent.angularSpeed = 0;
                _navMeshAgent.updateRotation = false;
            }

            transform.position = new Vector3(X, Y, Z);
            transform.localScale = Vector3.one;

            var anchor = transform.Find("Anchor");
            if (anchor != null)
            {
                var camera = AppManager.Instance.CurrScene.MainCamera;
                anchor.rotation = Quaternion.LookRotation(camera.transform.forward, Vector3.up);
            }

            InitSpine();
            InitStat();
            stateMachine.Init(OnActionChange);

            _agent.BlackboardReference.SetVariableValue("Self", gameObject);
            //ChangeState(ObjectState.Idle);
            //_Message.geometrySortingOrder = 100;// GlobalDefine.UI_SORTING_ORDER;
            stateMachine.PushState(WorldObjectActionType.Idle);
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


        public void Speak(string messge)
        {
            if (_Message != null)
            {
                _Message.text = messge;
            }
        }

        protected void OnActionChange(WorldObjectActionType actionType)
        {
            // React to event
            switch (actionType)
            {
                case WorldObjectActionType.None:
                    break;
                case WorldObjectActionType.Die:// 죽음 상태
                    Play("Die", false);
                    break;
                case WorldObjectActionType.Idle:// 일반 상태
                    Play("Idle", true);
                    break;
                case WorldObjectActionType.Patrol:// 순찰 상태
                    Play("Walk", true);
                    break;
                case WorldObjectActionType.Chase:// 추적 상태
                    Play("Run", true);
                    break;
                case WorldObjectActionType.Attack:// 공격 상태
                    Play("Attack1", false);
                    break;
                case WorldObjectActionType.Eat:// 먹기 상태
                    Play("Attack2", false);
                    break;
                case WorldObjectActionType.Sleep:// 잠자기 상태
                    Play("Attack2", false);
                    break;
            }
        }

        

        public void ChangeAction(WorldObjectActionType type)
        {
            stateMachine.PushState(type);
        }
    }


    /// <summary>
    /// 크리쳐 상태 머신.
    /// </summary>
    public class CretureStateMachine
    {
        //Stack<WorldObjectActionType> Stack = new Stack<WorldObjectActionType>();
        public Action<WorldObjectActionType> StateChange = null;
        private WorldObjectActionType State = WorldObjectActionType.None;

        public void Init(Action<WorldObjectActionType> callback)
        {
            StateChange = callback;
        }

        public void PushState(WorldObjectActionType state)
        {
            if (State == state)
                return;

            State = state;
            StateChange(State);
        }

        public WorldObjectActionType CurrentState()
        {
            return State;
        }
    }
}