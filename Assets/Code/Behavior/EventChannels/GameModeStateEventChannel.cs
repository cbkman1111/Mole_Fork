using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
#if UNITY_EDITOR

    [CreateAssetMenu(menuName = "Behavior/Event Channels/Game Mode State Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(
        name: "Game Mode State Event Channel",
        message: "Game Mode state has changed to [value]",
        category: "Events/Demo",
        id: "4837e8e4f38ae66a344cbf43469ef235")]
    public partial class GameModeStateEventChannel : EventChannelBase
    {
        public delegate void GameflowStateEventChannelEventHandler(GameModeState value);

        public event GameflowStateEventChannelEventHandler Event;

        public void SendEventMessage(GameModeState value)
        {
            Event?.Invoke(value);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<GameModeState> valueBlackboardVariable = messageData[0] as BlackboardVariable<GameModeState>;
            var value = valueBlackboardVariable != null ? valueBlackboardVariable.Value : default(GameModeState);

            Event?.Invoke(value);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            GameflowStateEventChannelEventHandler del = (value) =>
            {
                BlackboardVariable<GameModeState> var0 = vars[0] as BlackboardVariable<GameModeState>;
                if (var0 != null)
                    var0.Value = value;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as GameflowStateEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as GameflowStateEventChannelEventHandler;
        }
    }
}