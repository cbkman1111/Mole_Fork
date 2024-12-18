using Common.Utils;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Creature
{
    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    public partial class WorldObject : StateMachine
    {
        [SerializeField] 
        protected SkeletonAnimation _skel;
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
                GiantDebug.Log($"Animation '{name}' not found");
                return;
            }

            _skel.state.SetAnimation(0, name, loop);
        }

        protected virtual void HandleEventStart(TrackEntry trackEntry) {}
        protected virtual void HandleEventEnd(TrackEntry trackEntry) {}
        protected virtual void HandleEventCompete(TrackEntry trackEntry) {}
        protected virtual void HandleEvent(TrackEntry trackEntry, Spine.Event e) {}
    }
}