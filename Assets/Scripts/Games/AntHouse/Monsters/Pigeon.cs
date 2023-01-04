using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ant
{
    public class Pigeon : MonsterBase
    {
        protected enum SkellAnimationState { attack = 0, die, hit, idle, run, run_shoot };
        protected SkellAnimationState state = SkellAnimationState.idle;

        public SkeletonAnimation skel = null;
        protected override bool LoadSprite()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<SkeletonAnimation>("Ant/SpinePigeon");
            skel = Instantiate<SkeletonAnimation>(prefab, transform);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        protected void SetState(SkellAnimationState s)
        {
            if (s == state)
            {
                return;
            }

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
