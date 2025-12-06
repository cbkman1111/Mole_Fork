using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    public interface IMove
    {
        void Move(Vector3 direction);
        void Stop();
    }

    public class Creature : WorldObject, IMove
    {
        [HideInInspector] public Tween TweenMove { get; set; } = null;

        protected void Update()
        {
            // AI가 제어하는 상태(Chase, Patrol)일 때만 
            // NavMeshAgent의 실제 이동 방향(Velocity)에 맞춰 바라보는 방향을 갱신
            if (CurrentState == ObjectActionState.Chase || CurrentState == ObjectActionState.Patrol)
            {
                // 실제로 움직이고 있을 때만 회전 (제자리에서 미세하게 떨리는 현상 방지)
                if (NavMeshAgent != null && NavMeshAgent.velocity.sqrMagnitude > 0.1f)
                {
                    var velocityDir = NavMeshAgent.velocity.normalized;
                    UpdateDirection(velocityDir);
                }
            }
        }

        // --- IMove Interface Implementation (수동 조작) ---

        public void Move(Vector3 directionVector)
        {
            // [중요 수정] PlayAnimation -> SetActionState
            // 상태를 'Move'로 변경해야 Update 문 안의 AI 회전 로직과 충돌하지 않음
            SetActionState(ObjectActionState.Move);

            // 2. 방향 및 좌우 반전 갱신
            UpdateDirection(directionVector);

            // 3. 이동 처리 (NavMeshAgent 충돌 방지 로직)
            if (NavMeshAgent != null && NavMeshAgent.isOnNavMesh)
            {
                var speed = Stat.GetStat(Stat.StatType.Speed);
                // Agent.Move는 목적지가 아닌 '이동량(Delta)'을 입력받음
                NavMeshAgent.Move(directionVector * (speed * Time.deltaTime));
            }
            else
            {
                // NavMesh 밖이거나 에이전트가 없을 때는 Transform 이동
                var speed = Stat.GetStat(Stat.StatType.Speed);
                transform.Translate(directionVector * (speed * Time.deltaTime), Space.World);
            }
        }

        public void Stop()
        {
            if (TweenMove != null)
            {
                TweenMove.Kill();
                TweenMove = null;
            }

            // 이동 멈춤 처리
            if (NavMeshAgent != null && NavMeshAgent.isOnNavMesh)
            {
                NavMeshAgent.velocity = Vector3.zero;
                if (!NavMeshAgent.isStopped) NavMeshAgent.ResetPath();
            }

            // [중요 수정] 멈추면 Idle 상태로 복귀 (Stop 상태가 따로 있다면 Stop으로)
            SetActionState(ObjectActionState.Idle);
        }

        // --- Helper Methods ---

        /// <summary>
        /// 입력된 벡터를 기반으로 방향(Enum)과 스파인 좌우 반전을 처리
        /// </summary>
        private void UpdateDirection(Vector3 dirVector)
        {
            var newDir = GetDirect(dirVector);

            // 방향이 바뀌었을 때만 로직 수행 (최적화)
            if (Direction != newDir)
            {
                Direction = newDir;
                // OnDirectChanged(Direction); // 필요하다면 훅 메서드 호출
                UpdateFlip();
            }
        }

        /// <summary>
        /// 현재 Direction Enum에 따라 스케일(좌우) 반전
        /// </summary>
        private void UpdateFlip()
        {
            bool isLeft = (Direction & Direct.Left) != 0;
            bool isRight = (Direction & Direct.Right) != 0;

            Vector3 scale = transform.localScale;

            // 원본이 오른쪽을 보고 있다고 가정할 때의 로직
            if (isLeft) scale.x = Mathf.Abs(scale.x);        // 왼쪽이면 정방향 (스파인 원본 방향에 따라 +/- 조절 필요)
            else if (isRight) scale.x = -Mathf.Abs(scale.x); // 오른쪽이면 반전

            transform.localScale = scale;
        }
    }
}