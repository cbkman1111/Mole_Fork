using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ant
{
    public class Player : MonsterBase
    {
        public SkeletonAnimation skel = null;
        private bool loop = false;

        protected enum SkellAnimationState { die, idle, run, run_shoot };
        protected SkellAnimationState state = SkellAnimationState.idle;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool LoadSprite()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<SkeletonAnimation>("Ant/SpinePlayer");
            if (prefab == null)
            {
                return false;
            }

            skel = Instantiate<SkeletonAnimation>(prefab, transform);
            if (skel == null)
            {
                return false; 
            }

            skel.AnimationState.Event += HandleEvent;
            skel.AnimationState.Start += delegate (TrackEntry trackEntry) {
                // You can also use an anonymous delegate.
                Debug.Log(string.Format("track {0} started a new animation.", trackEntry.TrackIndex));
            };
            skel.AnimationState.End += delegate {
                // ... or choose to ignore its parameters.
                Debug.Log("An animation ended!");
                if (loop == false)
                {
                    SetState(SkellAnimationState.idle);
                }
            };
            return true;
        }

        void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            // Play some sound if the event named "footstep" fired.
            //if (e.Data.Name == footstepEventName)
            {
                Debug.Log($"HandleEvent {e.Data.Name}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector3 direction)
        {
            if (rigidBody != null)
            {
                hand.transform.position = transform.position + (direction * 0.6f);

                var position = transform.position + (direction * objData.speed);
                rigidBody.MovePosition(position);
            }

            SetState(SkellAnimationState.run);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            SetState(SkellAnimationState.idle);
        }

        /// <summary>
        /// Å¸ÀÏ ±ú±â
        /// </summary>
        public void Hit()
        {
            SetState(SkellAnimationState.run_shoot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        protected void SetState(SkellAnimationState s)
        {
            if (s == state)
                return;

            state = s;
            string name = SkellAnimationState.idle.ToString();
            switch (state)
            {
                case SkellAnimationState.idle:
                case SkellAnimationState.run:
                case SkellAnimationState.die:
                    loop = true;
                    break;
                case SkellAnimationState.run_shoot:
                    loop = false;
                    break;
            }

            name = state.ToString();
            skel.state.SetAnimation(0, name, loop);
        }
    }
}
