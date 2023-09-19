using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TileMap
{
    public class Animal : Creature
    {
        private bool idle = true;
        
        public virtual void Move(Vector3 angle)
        {
            if (idle == true)
            {
                idle = false;
                skel.state.SetAnimation(0, "run", true);
            }

            var speed = 0.25f;
            var target = transform.position + (angle * speed);
            target.y = 0;

            transform.DOKill();
            transform.DOMove(target, 0.1f).
                OnUpdate(() =>
                {
                    
                }).
                SetEase(Ease.Linear).
                OnComplete(() =>
                {
                    idle = true;
                    skel.state.SetAnimation(0, "idle", true);
                });
        }
    }
}
