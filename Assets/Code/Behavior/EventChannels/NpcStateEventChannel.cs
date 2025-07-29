using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/NPC State Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(
        name: "NPC State Event Channel",
        message: "NPC state has changed to [value]",
        category: "Events/Demo",
        id: "bc071d55c8ec6eb442f68dc82bafbadb")]
    public partial class NpcStateEventChannel : EventChannelBase
    {
        public delegate void GuardStateEventChannelEventHandler(NpcState value);

        public event GuardStateEventChannelEventHandler Event;

        public void SendEventMessage(NpcState value)
        {
            Event?.Invoke(value);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<NpcState> valueBlackboardVariable = messageData[0] as BlackboardVariable<NpcState>;
            var value = valueBlackboardVariable != null ? valueBlackboardVariable.Value : default(NpcState);

            Event?.Invoke(value);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            GuardStateEventChannelEventHandler del = (value) =>
            {
                BlackboardVariable<NpcState> var0 = vars[0] as BlackboardVariable<NpcState>;
                if (var0 != null)
                    var0.Value = value;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as GuardStateEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as GuardStateEventChannelEventHandler;
        }
    }
}