using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Common.Global;
using UnityEngine;
using UnityEngine.AI;

namespace Ant
{
    public class Pigeon : MonsterBase
    {
        protected override bool LoadSprite()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<SkeletonAnimation>("SpinePigeon");
            _skel = Instantiate<SkeletonAnimation>(prefab, transform);

            if (_skel == null)
            {
                return false;
            }

            _skel.AnimationState.Event += HandleEvent;
            _skel.AnimationState.Start += delegate (TrackEntry trackEntry) {
                Debug.Log(string.Format("track {0} started a new animation.", trackEntry.TrackIndex));
            };
            _skel.AnimationState.End += delegate {
                // ... or choose to ignore its parameters.
                Debug.Log("An animation ended!");
                if (loop == false)
                {
                    SetState(TileMap.ObjectState.Idle);
                }
            };

            return true;
        }

    }
}
