using DG.Tweening;
using Spine;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    public class Animal : Creature
    {

        /// <summary>
        /// ¿Ï·á.
        /// </summary>
        /// <param name="trackEntry"></param>
        protected override void HandleEventCompete(TrackEntry trackEntry)
        {
            if (State == ObjectState.Stop)
            {
                ChangeState(ObjectState.Idle);
            }
        }
    }
}
