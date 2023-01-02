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
        protected enum SkellAnimationState { attack = 0, die, hit, idle, run, run_shoot };
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

            return true;
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
            SetState(SkellAnimationState.attack);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        protected void SetState(SkellAnimationState s)
        {
            if (s == state)
                return;

            string name = SkellAnimationState.idle.ToString();
            switch (state)
            {
                case SkellAnimationState.idle:
                case SkellAnimationState.attack:
                case SkellAnimationState.die:
                case SkellAnimationState.hit:
                case SkellAnimationState.run:
                case SkellAnimationState.run_shoot:
                    name = s.ToString();
                    break;
            }

            state = s;
            skel.state.SetAnimation(0, name, true);
        }
    }
}
