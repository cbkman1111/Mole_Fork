using System;
using Common.Global;
using Games.BehaviorTree.Datas;
using Games.TileMap.Datas;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    
    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    public partial class WorldObject : StateMachine
    //public partial class WorldObject : MonoBehaviour
    {
        [SerializeField] protected NavMeshAgent NavMeshAgent;
        [SerializeField] protected BehaviorGraphAgent BehaviorAgent = null;
        public BlackboardReference BlackboardReference => BehaviorAgent.BlackboardReference;
        //protected CretureStateMachine stateMachine = new CretureStateMachine();

        [SerializeField] protected CharacterHud Hud = null;

        [System.Flags]
        public enum Direct
        {
            None = 0,
            Up = 1 << 0, // 0001
            Down = 1 << 1, // 0010
            Left = 1 << 2, // 0100
            Right = 1 << 3  // 1000
        }

        public Games.TileMap.Datas.Coordinate Coordinate = new();

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
            Coordinate.X = x;
            Coordinate.Z = z;
            //Coordinate.Y = 0;

            if (NavMeshAgent != null)
            {
                NavMeshAgent.angularSpeed = 0;
                NavMeshAgent.updateRotation = false;
            }

            transform.position = Coordinate.Position;
            transform.localScale = Vector3.one;

            var anchor = transform.Find("Anchor");
            if (anchor != null)
            {
                var camera = AppManager.Instance.CurrScene.MainCamera;
                anchor.rotation = Quaternion.LookRotation(camera.transform.forward, Vector3.up);
            }

            InitSpine();
            InitStat();
            InitHud();

            BehaviorAgent.BlackboardReference.SetVariableValue("Self", gameObject);
            ChangeState(ObjectActionState.Idle);
            //_Message.geometrySortingOrder = 100;// GlobalDefine.UI_SORTING_ORDER;
            //stateMachine.PushState(WorldObjectActionType.Idle);
            return true;
        }

        public void InitStat()
        {
            Stat.Health = 100;   
        }

        public void InitHud()
        {
            Hud.Init();
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


        public void Speak(string msg)
        {
            if(Hud == null)
                return;

            Hud.SetMessage(msg);
        }


        public override void OnStateEnter(ObjectActionState state)
        {            // React to event
            switch (state)
            {
                case ObjectActionState.None:
                    break;
                case ObjectActionState.Idle:// 일반 상태
                    Play("Idle", true);
                    break;
                case ObjectActionState.Attack:// 공격 상태
                    Play("Attack1", false);
                    break;
                case ObjectActionState.Patrol:// 순찰 상태
                    Play("Walk", true);
                    break;
                case ObjectActionState.Chase:// 추적 상태
                    Play("Run", true);
                    break;


                    /*
                case ObjectActionState.Die:// 죽음 상태
                    Play("Die", false);
                    break;

                case ObjectActionState.Eat:// 먹기 상태
                    Play("Attack2", false);
                    break;
                case ObjectActionState.Sleep:// 잠자기 상태
                    Play("Attack2", false);
                    break;
                    */
            }
        }

        public override void OnStateExit(ObjectActionState state) 
        { 
        }

        /*
        public void ChangeAction(WorldObjectActionType type)
        {
            stateMachine.PushState(type);
        }
        */
    }
}