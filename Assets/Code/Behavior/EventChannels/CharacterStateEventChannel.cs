using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
#if UNITY_EDITOR

    [CreateAssetMenu(menuName = "Behavior/Event Channels/Character State Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(
        name: "Character State Event Channel", 
        message: "Character state has changed to [newState]", 
        category: "Events/Demo", 
        id: "6778ae8f566a0ba66723320654ad904e")]
    public partial class CharacterStateEventChannel : EventChannelBase
    {
        public delegate void CharacterStateEventEventHandler(CharacterState newState);

        public event CharacterStateEventEventHandler Event;

        public void SendEventMessage(CharacterState state)
        {
            Event?.Invoke(state);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<CharacterState> stateBlackboardVariable = messageData[0] as BlackboardVariable<CharacterState>;
            var state = stateBlackboardVariable != null ? stateBlackboardVariable.Value : default(CharacterState);

            Event?.Invoke(state);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            CharacterStateEventEventHandler del = (state) =>
            {
                BlackboardVariable<CharacterState> var0 = vars[0] as BlackboardVariable<CharacterState>;
                if (var0 != null)
                    var0.Value = state;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as CharacterStateEventEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as CharacterStateEventEventHandler;
        }
    }
}