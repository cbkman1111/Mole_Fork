using UnityEngine;

namespace Creature
{
    public enum ObjectState
    {
        None = 0,
        Idle,

        Move,
        Stop,
        Hit,
        Attack,
        Dead,

        Click, // 클릭했을때 테스트용.
    }

    /// <summary>
    /// 상태머신.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StateMachine : MonoBehaviour
    {
        protected ObjectState State = ObjectState.None;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="change"></param>
        public void ChangeState(ObjectState change)
        {
            if(State == change)
                return;

            OnExitState(State);

            State = change;

            OnEnterState(State);
        }

        public abstract void OnEnterState(ObjectState state);
        public abstract void OnExitState(ObjectState state);
    }
}
