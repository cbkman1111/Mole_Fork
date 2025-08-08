using System;
using DG.Tweening;
using UnityEngine;

namespace Creature
{
    public interface IMove
    {
        public void Move(Vector3 angle);
        public void Stop();
    }

    public class Creature : WorldObject, IMove
    {
        [HideInInspector] public Tween TweenMove { get; set; } = null;

        public void Move(Vector3 angle)
        {
            ChangeState(ObjectState.Move);

            var before = Direction;
            var curr = GetDirect(angle);
            if (before != curr)
            {
                Direction = curr;
                OnDirectChanged(Direction);
            }

            var speed = Stat.GetStat(Stat.StatType.Speed);
            var target = angle * speed * Time.deltaTime;
            target.y = 0.0f;

            transform.Translate(target, Space.World);
        }

        public void Stop()
        {
            if (TweenMove != null)
            {
                TweenMove.Kill();
                TweenMove = null;
            }

            ChangeState(ObjectState.Stop);
        }

        private void UpdateStateAnimation()
        {
            switch (State)
            {
                case ObjectState.Stop:
                case ObjectState.Idle:
                    Play("Idle", true);
                    break;

                case ObjectState.Move:
                    var speed = Stat.GetStat(Stat.StatType.Speed);
                    if (speed >= 3)
                    {
                        Play("Run", true);
                    }
                    else
                    {
                        Play("Walk", true);
                    }
                    break;
            }

            bool isLeft = (Direction & Direct.Left) != 0;  // true
            bool isRight = (Direction & Direct.Right) != 0; // false
            bool isUp = (Direction & Direct.Up) != 0;      // false
            bool isDown = (Direction & Direct.Down) != 0;  // false

            Vector3 flip = Vector3.one;
            if (isLeft == true)
                flip.x = 1;
            else if (isRight == true)
                flip.x = -1;

            _skel.transform.localScale = flip;
        }

        /// <summary>
        /// 다른 상태로 바뀜.
        /// </summary>
        /// <param name="state"></param>
        public override void OnStateEnter(ObjectState state)
        {
            UpdateStateAnimation();

            switch (state)
            {
                case ObjectState.Idle:
                case ObjectState.Move:
                    break;

                case ObjectState.Stop:
                    break;

                case ObjectState.Click:
                    transform.DOPunchScale(transform.forward, 0.1f).
                        OnComplete(() => {
                            ChangeState(ObjectState.Idle);
                        });
                    break;

                case ObjectState.Dead:
                    break;
            }
        }

        protected virtual void OnDirectChanged(Direct direct) 
        {
            UpdateStateAnimation();
        }
    }
}
