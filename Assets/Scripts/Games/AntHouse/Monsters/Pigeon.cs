using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ant
{
    public class Pigeon : MonsterBase
    {
        public SkeletonAnimation skel = null;
        protected override bool LoadSprite()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<SkeletonAnimation>("Ant/SpinePigeon");
            skel = Instantiate<SkeletonAnimation>(prefab, transform);

            return true;
        }
    }
}
