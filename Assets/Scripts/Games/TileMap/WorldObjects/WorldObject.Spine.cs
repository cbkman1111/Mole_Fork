using Common.Utils;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Creature
{
    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    //public partial class WorldObject : StateMachine
    public partial class WorldObject : MonoBehaviour //StateMachine
    {
        [SerializeField] protected SkeletonAnimation _skel;
        [SerializeField] protected Rigidbody _rigidbody;

        public SkeletonAnimation Skel => _skel;
        private bool _isSpineInitialized = false;
        public bool InitSpine()
        {
            if (_skel == null) 
                return false;

            if (_isSpineInitialized)
            {
                UnsubscribeSpineEvents();
            }

            // 이벤트 구독
            if (_skel.AnimationState != null)
            {
                _skel.AnimationState.Event += HandleEvent;
                _skel.AnimationState.Complete += HandleEventCompete;
                _skel.AnimationState.Start += HandleEventStart;
                _skel.AnimationState.End += HandleEventEnd;
            }

            _isSpineInitialized = true;
            return true;
        }

        // [추가] 객체 파괴 시 이벤트 해제 필수 (메모리 릭 방지)
        protected virtual void OnDestroy()
        {
            UnsubscribeSpineEvents();
        }

        private void UnsubscribeSpineEvents()
        {
            if (_skel == null || _skel.AnimationState == null) return;

            _skel.AnimationState.Event -= HandleEvent;
            _skel.AnimationState.Complete -= HandleEventCompete;
            _skel.AnimationState.Start -= HandleEventStart;
            _skel.AnimationState.End -= HandleEventEnd;
        }


        // [연결 함수] Enum 상태를 스파인 애니메이션 이름(string)으로 변환하여 재생
        protected virtual void PlayAnimation(ObjectActionState state)
        {
            switch (state)
            {
                case ObjectActionState.Idle:
                    Play("Idle", true);
                    break;

                case ObjectActionState.Move:
                case ObjectActionState.Chase: // 추적 시 이동 모션
                case ObjectActionState.Patrol: // 순찰 시 이동 모션
                    // 필요하다면 여기서 속도(Stat.Speed)를 체크해서 Walk/Run 분기 가능
                    Play("Run", true);
                    break;

                case ObjectActionState.Attack:
                    Play("Attack1", false);
                    break;

                case ObjectActionState.Die:
                    Play("Die", false);
                    break;

                    // 필요한 상태 케이스 추가...
            }
        }

        /// <summary>
        /// 애니메이션 재생.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loop"></param>
        protected void Play(string name, bool loop = false)
        {
            if (_skel == null || _skel.AnimationState == null) return;

            // 1. 애니메이션 객체를 먼저 찾습니다. (유효성 검사)
            var targetAnim = _skel.skeleton.Data.FindAnimation(name);
            if (targetAnim == null)
            {
                // 빈번하게 발생하는 로그라면 성능을 위해 조건부 컴파일이나 레벨 조정 필요
                GiantDebug.LogError($"[Spine] Animation '{name}' not found on {_skel.name}");
                return;
            }

            // 2. 현재 재생 중인 트랙 확인
            var currentTrack = _skel.AnimationState.GetCurrent(0);

            // [최적화] 문자열 비교(name == name) 대신 객체 포인터 비교(ReferenceEquals) 사용
            // 같은 애니메이션이 계속 재생 요청될 때의 부하를 최소화
            if (currentTrack != null && currentTrack.Animation == targetAnim)
            {
                // 이미 돌고 있는 애니메이션이고, 루프 상태도 같다면 리턴
                if (currentTrack.Loop == loop && !currentTrack.IsComplete)
                    return;
            }

            // 3. 이름(string) 대신 찾은 객체(Animation)를 바로 넣어서 내부 검색 비용 절약
            _skel.AnimationState.SetAnimation(0, targetAnim, loop);
        }

        public bool IsSpineAnimationPlaying()
        {
            if (_skel == null || _skel.AnimationState == null) return false;

            TrackEntry trackEntry = _skel.AnimationState.GetCurrent(0);

            // Loop 중이거나, 아직 끝나지 않았으면 재생 중으로 판단
            // (TimeScale이 0일 경우도 고려할 수 있으나 기본적으로는 이 정도면 충분)
            return trackEntry != null && (trackEntry.Loop || !trackEntry.IsComplete);
        }

        // --- Event Handlers ---
        protected virtual void HandleEventStart(TrackEntry trackEntry) { }
        protected virtual void HandleEventEnd(TrackEntry trackEntry) { }
        protected virtual void HandleEventCompete(TrackEntry trackEntry) { }

        // Spine.Event는 유니티 Event와 이름이 겹칠 수 있으므로 풀네임 권장
        protected virtual void HandleEvent(TrackEntry trackEntry, Spine.Event e) { }
    }
}