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
    //public partial class WorldObject : StateMachine
    public partial class WorldObject : MonoBehaviour
    {
        public ObjectActionState CurrentState { get; private set; } = ObjectActionState.Idle;

        [SerializeField] protected NavMeshAgent NavMeshAgent;
        [SerializeField] protected BehaviorGraphAgent BehaviorAgent;
        [SerializeField] protected CharacterHud Hud;

        // [변경 2] Find("Anchor") 제거 -> 인스펙터 할당 권장
        [SerializeField] protected Transform AnchorPoint;
        public BlackboardReference BlackboardReference => BehaviorAgent != null ? BehaviorAgent.BlackboardReference : null;

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

        [HideInInspector] public Direct Direction { get; protected set; } = Direct.Down;

        /// <summary>
        /// 스탯.
        /// </summary>
        public Stat Stat { get; set; } = new Stat();

        public static WorldObject Create(string path, Transform parent, int x, int z)
        {
            // [변경 4] 리소스 로드 로직 명확화
            // LoadInBuild<GameObject>가 프리팹을 리턴한다고 가정합니다.
            var prefab = ResourcesManager.Instance.LoadBundle(path);

            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab: {path}");
                return null;
            }

            // 인스턴스 생성
            var go = Instantiate(prefab, parent);
            go.name = prefab.name;

            var worldObj = go.GetComponent<WorldObject>();
            // [수정 3] 컴포넌트가 없거나 초기화 실패 시 처리
            if (worldObj == null)
            {
                Debug.LogError($"[WorldObject] Prefab '{path}' does not have 'WorldObject' component!");
                Destroy(go); // 껍데기만 남은 오브젝트 파괴
                return null;
            }

            if (worldObj.Init(x, z))
            {
                return worldObj;
            }

            // 초기화 실패 시 파괴
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

            // 빌보드 처리
            if (AnchorPoint != null)
            {
                var mainCam = Camera.main; // 캐싱된 카메라 매니저가 있다면 그걸 사용
                if (mainCam != null)
                {
                    AnchorPoint.rotation = Quaternion.LookRotation(mainCam.transform.forward, Vector3.up);
                }
            }

            InitSpine();
            InitStat();
            InitHud();

            Play("Idle", true);

            if (BlackboardReference != null)
                BlackboardReference.SetVariableValue("Self", gameObject);

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

        /// <summary>
        /// Behavior Graph의 Action 노드나 외부에서 호출하여 상태를 갱신
        /// </summary>
        public void SetActionState(ObjectActionState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;

            // 상태가 바뀔 때 애니메이션 갱신
            // WorldObject.spine 파셜 클래스의 Play(string, bool)을 호출하기 위한 연결 함수
            PlayAnimation(CurrentState);
        }

        public void Speak(string msg)
        {
            if(Hud == null)
                return;

            Hud.SetMessage(msg);
        }
    }
}