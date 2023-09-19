using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TileMap
{
    public class Animal : Creature
    {
        public virtual void Move(Vector3 angle)
        {
            var speed = 0.5f;
            var target = transform.position + (angle * speed);
            target.y = 0;
            transform.DOMove(target, 0.1f).
                OnUpdate(() =>
                {

                }).
                SetEase(Ease.Linear).
                OnComplete(() =>
                {
                    
                });
        }
    }
}
