using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
#if UNITY_EDITOR

    [CreateAssetMenu(menuName = "Behavior/Event Channels/Game Pause Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(
        name: "Game Pause Event Channel", 
        message: "Game pause state: [Value]", 
        category: "Events/Demo", 
        id: "8b6fdd50d1d561eb66608f652d7ec6c7")]
    public partial class GamePauseEventChannel : EventChannelBase
    {
        public delegate void GamePauseEventChannelEventHandler(bool Value);

        public event GamePauseEventChannelEventHandler Event;

        public void SendEventMessage(bool Value)
        {
            Event?.Invoke(Value);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<bool> ValueBlackboardVariable = messageData[0] as BlackboardVariable<bool>;
            var Value = ValueBlackboardVariable != null ? ValueBlackboardVariable.Value : default(bool);

            Event?.Invoke(Value);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            GamePauseEventChannelEventHandler del = (Value) =>
            {
                BlackboardVariable<bool> var0 = vars[0] as BlackboardVariable<bool>;
                if (var0 != null)
                    var0.Value = Value;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as GamePauseEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as GamePauseEventChannelEventHandler;
        }
    }
}