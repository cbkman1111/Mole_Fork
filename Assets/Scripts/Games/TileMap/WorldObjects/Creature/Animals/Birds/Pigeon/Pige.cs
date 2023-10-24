using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace TileMap
{
    public class Pige : Bird
    {
        [SerializeField] protected SkeletonAnimation _skel;

        public virtual void Move(Vector3 angle)
        {
            if (_stateMachine.State == ObjectState.Idle)
            {
                _stateMachine.PushState(ObjectState.Move);
            }
            else if(_stateMachine.State == ObjectState.Move)
            {
                var speed = 0.15f;
                var target = transform.position + (angle * speed);
                target.y = 0.5f;

                if(angle.x < 0f)
                    _skel.skeleton.ScaleX = -1;
                else
                    _skel.skeleton.ScaleX = 1;
                
                transform.DOKill();
                transform.DOMove(target, 0.1f).
                    OnUpdate(() =>
                    {
                    }).
                    SetEase(Ease.Linear).
                    OnComplete(() =>
                    {
                        _stateMachine.PushState(ObjectState.Stop);
                    });
            }
            else if (_stateMachine.State == ObjectState.Click)
            {
               
            }
        }
        
        protected override void OnEnterState(ObjectState before, ObjectState after)
        {
            switch (after)
            {
                case ObjectState.Idle:
                    _skel.state.SetAnimation(0, "idle_1", true);
                    break;
                case ObjectState.Move:
                    _skel.state.SetAnimation(0, "run", true);
                    break;
                case ObjectState.Stop:
                    //_skel.state.SetAnimation(0, "idle", true);
                    _stateMachine.PushState(ObjectState.Idle);
                    break;
                case ObjectState.Click:
                    _skel.state.SetAnimation(0, "sword_attack", false);
                    _stateMachine.PushState(ObjectState.Idle);
                    break;
            }
        }
    }
}
