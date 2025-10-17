using UnityEngine;

namespace Creature
{
    /// <summary>
    /// 상태머신.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StateMachine : MonoBehaviour
    {
        protected ObjectActionState State = ObjectActionState.None;

        public void ChangeState(ObjectActionState change)
        {
            if(State == change)
                return;

            OnStateExit(State);

            State = change;

            OnStateEnter(State);
        }

        public abstract void OnStateEnter(ObjectActionState state);
        public abstract void OnStateExit(ObjectActionState state);
    }
}
