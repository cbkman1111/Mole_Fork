using Common.Global;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Ant
{
    public class Player : MonsterBase
    {
        public void SetScale(Vector3 scale)
        {
            _skel.transform.localScale = scale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool LoadSprite()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<SkeletonAnimation>("SpinePlayer");
            if (prefab == null)
            {
                return false;
            }

            _skel = Instantiate<SkeletonAnimation>(prefab, transform);
            if (_skel == null)
            {
                return false; 
            }

            _skel.AnimationState.Event += HandleEvent;
            _skel.AnimationState.Start += delegate (TrackEntry trackEntry) {
                // You can also use an anonymous delegate.
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
