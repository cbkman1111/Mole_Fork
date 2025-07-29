using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
#if UNITY_EDITOR

    [CreateAssetMenu(menuName = "Behavior/Event Channels/GameState Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(
        name: "GameState Event Channel",
        message: "Game state has changed to [Value]", 
        category: "Events/Demo", 
        id: "9bd2734d8647eace43b343b0ba21a1af")]
    public partial class GameStateEventChannel : EventChannelBase
    {
        public delegate void GameStateEventChannelEventHandler(GameState Value);

        public event GameStateEventChannelEventHandler Event;

        public void SendEventMessage(GameState Value)
        {
            Event?.Invoke(Value);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<GameState> ValueBlackboardVariable = messageData[0] as BlackboardVariable<GameState>;
            var Value = ValueBlackboardVariable != null ? ValueBlackboardVariable.Value : default(GameState);

            Event?.Invoke(Value);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            GameStateEventChannelEventHandler del = (Value) =>
            {
                BlackboardVariable<GameState> var0 = vars[0] as BlackboardVariable<GameState>;
                if (var0 != null)
                    var0.Value = Value;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as GameStateEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as GameStateEventChannelEventHandler;
        }
    }
}