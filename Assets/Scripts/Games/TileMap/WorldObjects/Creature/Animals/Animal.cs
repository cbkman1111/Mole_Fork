using DG.Tweening;
using Spine;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    public interface IMove
    {
        public void Move(Vector3 angle);
        public void Dash(Vector3 angle);
        public void Stop();
    }

    public class Animal : Creature, IMove
    {
        public enum Direct
        {
            None = 0,

            Up,
            Down,
            Left,
            Right,
        }

        [HideInInspector]
        public Direct direct { get; set; } = Direct.Down;
        
        [HideInInspector]
        public Tween TweenMove { get; set; } = null;

        protected Direct GetDirect(Vector3 angle)
        {
            Direct d = Direct.None;
            if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x))
            {
                if (angle.z > 0f)
                    d = Direct.Up;
                else if (angle.z < 0f)
                    d = Direct.Down;
            }
            else
            {
                if (angle.x > 0f)
                    d = Direct.Left;
                else
                    d = Direct.Right;
            }

            return d;
        }

        public void Move(Vector3 angle) 
        {
            ChangeState(ObjectState.Move);

            var before = direct;
            var curr = GetDirect(angle);
            if (before != curr)
            {
                direct = curr;
                OnDirectChanged(direct);
            }

            //var duration = 0.1f;
            var speed = 2f;
            var target = angle * speed * Time.deltaTime;
            target.y = 0.0f;

            //_navMeshAgent.speed = speed;
            //_navMeshAgent.updateRotation = false; // 회전은 직접 처리함.
            //_navMeshAgent.SetDestination(target);// = target;
            transform.Translate(target, Space.World);
            /*
            transform.DOKill();
            TweenMove = transform.DOMove(transform.position + target, duration).
                SetEase(Ease.Linear).
                OnComplete(() => {
                    TweenMove = null;
                    //ChangeState(ObjectState.Idle);
                });
            */
        }

        public void Dash(Vector3 angle) 
        {
            //OnDash(angle);

            //var duration = 0.4f;
            var speed = 1;
            var target = angle.normalized * speed * Time.deltaTime;
            target.y = 0.0f;

            //_rigidbody.linearVelocity = target;
            //_navMeshAgent.velocity = target;
            //transform.Translate(target, Space.World);
            /*
            transform.DOKill();
            TweenMove = transform.DOMove(transform.position + target, duration).
                SetEase(Ease.Linear).
                OnComplete(() => {
                    TweenMove = null;
                    ChangeState(ObjectState.Stop);
                });
            */
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

        /// <summary>
        /// 완료.
        /// </summary>
        /// <param name="trackEntry"></param>
        protected override void HandleEventCompete(TrackEntry trackEntry)
        {
            if (State == ObjectState.Stop)
            {
                ChangeState(ObjectState.Idle);
            }
        }

        protected virtual void OnDash(Vector3 angle) {}
        protected virtual void OnDirectChanged(Direct direct) { }
    }
}
