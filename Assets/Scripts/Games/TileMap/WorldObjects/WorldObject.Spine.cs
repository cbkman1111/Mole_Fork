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
        
        public bool InitSpine()
        {
            if (_skel != null)
            {
                _skel.Initialize(true);
                _skel.AnimationState.Event += HandleEvent;
                _skel.AnimationState.Complete += HandleEventCompete;
                _skel.AnimationState.Start += HandleEventStart;
                _skel.AnimationState.End += HandleEventEnd;
            }

            return true;
        }

        /// <summary>
        /// 애니메이션 재생.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loop"></param>
        protected void Play(string name, bool loop = false)
        {
            if (_skel == null)
                return;
            
            var animation = _skel.skeleton.Data.FindAnimation(name);
            if (animation == null)
            {
                GiantDebug.LogError($"Animation '{name}' not found");
                return;
            }
            
            var trackEntry = _skel.state.GetCurrent(0); // 0번 트랙(기본 트랙)
            if (trackEntry != null && trackEntry.Animation != null)
            {
                string animationName = trackEntry.Animation.Name;
                if (animationName == name)
                    return;
            }

            _skel.state.SetAnimation(0, name, loop);
        }

        public bool IsSpineAnimationPlaying()
        {
            TrackEntry trackEntry = _skel.AnimationState.GetCurrent(0);
            return (trackEntry != null && (trackEntry.Loop || !trackEntry.IsComplete));
        }

        protected virtual void HandleEventStart(TrackEntry trackEntry) {}
        protected virtual void HandleEventEnd(TrackEntry trackEntry) {}
        protected virtual void HandleEventCompete(TrackEntry trackEntry) {}
        protected virtual void HandleEvent(TrackEntry trackEntry, Spine.Event e) {}
    }
}