using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace TileMap
{   
    public enum ObjectState
    {
        None = 0,
            
        Idle,

        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,

        Stop,

        Hit,
        Attack,
        
        Click, // 클릭했을때 테스트용.
    }
    
    public class StateMachine<T>
    {
        private Stack<T> _stack = new Stack<T>();
        public T State => _stack.Count > 0 ? _stack.Peek() : default(T);
        
        public Action<T, T> OnEnterState = null;
        public Action<T> OnPopState = null;

        public void PushState(T change)
        {
            if (_stack.Contains(change) == true)
                return;

            var before = PopState();
            _stack.Push(change);
            OnEnterState?.Invoke(before, change);
        }

        public T PopState()
        {
            if (_stack.Count > 0)
            {            
                T before = _stack.Pop();
                OnPopState?.Invoke(before);
                return before;
            }
            else
            {
                return default(T);
            }
        }
    }

    /// <summary>
    /// 모든 맵위의 객체들의 기본값.
    /// </summary>
    public class WorldObject : MonoBehaviour
    {
        protected StateMachine<ObjectState> _stateMachine = new StateMachine<ObjectState>();

        [SerializeField] 
        protected SkeletonAnimation _skel;
        public SkeletonAnimation Skel => _skel;

        /// <summary>
        /// 타일의 좌표계.
        /// </summary>
        public int x;
        public int z;
        
        public bool Init(int posX, int posZ, Vector3 scale)
        {
            x = posX;
            z = posZ;
            
            SetState(ObjectState.Idle);

            transform.position = new Vector3(x, 0, z);
            _stateMachine.OnEnterState = OnEnterState;
            _stateMachine.OnPopState = OnPopState;
            _skel?.Initialize(true);
            return true;
        }

        public void SetState(ObjectState state)
        {
            _stateMachine.PushState(state);
        }

        private void StartAnimation(string name)
        {
            var animation = _skel.skeleton.Data.FindAnimation(name);
            if (animation == null)
            {
                Debug.Log($"Animation '{name}' not found");
                return;
            }

            _skel.state.SetAnimation(0, name, true);
        }

        protected virtual void OnEnterState(ObjectState before, ObjectState after)
        {
            switch (after)
            {
                case ObjectState.Idle:
                    StartAnimation("movement/idle-front");
                    //_skel.state.SetAnimation(0, "movement/idle-front", true);
                    break;
                case ObjectState.MoveLeft:
                    StartAnimation("movement/trot-left");
                    //_skel.state.SetAnimation(0, "movement/trot-left", true);
                    break;
                case ObjectState.MoveRight:
                    StartAnimation("movement/trot-right");
                    //_skel.state.SetAnimation(0, "movement/trot-right", true);
                    break;
                case ObjectState.MoveUp:
                    StartAnimation("movement/trot-back");
                    //_skel.state.SetAnimation(0, "movement/trot-back", true);
                    break;
                case ObjectState.MoveDown:
                    StartAnimation("movement/trot-front");
                    //_skel.state.SetAnimation(0, "movement/trot-front", true);
                    break;

                case ObjectState.Stop:
                    StartAnimation("emotes/sulk");
                    //_skel.state.SetAnimation(0, "idle", true);
                    //_skel.state.SetAnimation(0, "emotes/sulk", false);
                    _stateMachine.PushState(ObjectState.Idle);
                    break;
                case ObjectState.Click:
                    //skel.state.SetAnimation(0, "emotes/angry", false);
                    //_stateMachine.PushState(ObjectState.Idle);
                    if (after == ObjectState.Click) {
                        transform.DOPunchScale(transform.forward, 0.1f).OnComplete(() => {
                            SetState(ObjectState.Idle);
                        });
                    }
                       
                    break;
            }
        }
        protected virtual void OnPopState(ObjectState state)
        {
        }

       
        public virtual void Move(Vector3 angle)
        {
            switch (_stateMachine.State)
            {
                case ObjectState.Idle:

                    if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x))
                    {
                        if (angle.z > 0f)
                            _stateMachine.PushState(ObjectState.MoveUp);
                        else if (angle.z < 0f)
                            _stateMachine.PushState(ObjectState.MoveDown);
                    }
                    else
                    {
                        if (angle.x > 0f)
                            _stateMachine.PushState(ObjectState.MoveLeft);
                        else
                            _stateMachine.PushState(ObjectState.MoveRight);
                    }

                    break;
                case ObjectState.MoveLeft:
                case ObjectState.MoveRight:
                case ObjectState.MoveUp:
                case ObjectState.MoveDown:

                    if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x))
                    {
                        if (angle.z > 0f)
                            _stateMachine.PushState(ObjectState.MoveUp);
                        else if (angle.z < 0f)
                            _stateMachine.PushState(ObjectState.MoveDown);
                    }
                    else
                    {
                        if (angle.x > 0f)
                            _stateMachine.PushState(ObjectState.MoveLeft);
                        else
                            _stateMachine.PushState(ObjectState.MoveRight);
                    }

                    var speed = 0.10f;
                    var target = transform.position + (angle * speed);
                    target.y = 0.0f;

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
                    break;

                case ObjectState.Stop:

                    break;
                case ObjectState.Click:
                    break;
            }
        }
    }
}


