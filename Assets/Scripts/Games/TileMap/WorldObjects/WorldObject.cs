using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TileMap
{   
    public enum ObjectState
    {
        None = 0,
            
        Idle,
        Move,
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
       
        /// <summary>
        /// 타일의 좌표계.
        /// </summary>
        public int x;
        public int z;
        
        public bool Init(int posX, int y, int posZ)
        {
            x = posX;
            z = posZ;
            
            transform.position = new Vector3(x, .5f, z);
            _stateMachine.OnEnterState = OnEnterState;
            _stateMachine.OnPopState = OnPopState;
            
            SetState(ObjectState.Idle);
            return true;
        }

        public void SetState(ObjectState state)
        {
            _stateMachine.PushState(state);
        }

        protected virtual void OnEnterState(ObjectState before, ObjectState after)
        {
            if (after == ObjectState.Click)
            {
                transform.DOPunchScale(transform.forward, 0.1f).OnComplete(() =>
                {
                    SetState(ObjectState.Idle);
                });
            }
        }
        
        protected virtual void OnPopState(ObjectState state)
        {
        }
    }
}


