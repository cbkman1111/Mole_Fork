using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ant
{
    public class Pigeon : MonsterBase
    {
        protected override bool LoadSprite()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<SkeletonAnimation>("SpinePigeon");
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

    }
}
