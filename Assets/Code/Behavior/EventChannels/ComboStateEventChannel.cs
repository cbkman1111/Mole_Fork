using System;
using Unity.Properties;

namespace Unity.Behavior.Demo
{
#if UNITY_EDITOR
    [UnityEngine.CreateAssetMenu(menuName = "Behavior/Event Channels/Combo State Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(
        name: "Combo State Event Channel", 
        message: "Combo state has changed to [value]", 
        category: "Events", 
        id: "0d1a6905b3e0a7515b7c859053e1aeff")]
    public partial class ComboStateEventChannel : EventChannelBase
    {
        public delegate void ComboStateEventChannelEventHandler(ComboState value);
        public event ComboStateEventChannelEventHandler Event;

        public void SendEventMessage(ComboState value)
        {
            Event?.Invoke(value);
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            BlackboardVariable<ComboState> valueBlackboardVariable = messageData[0] as BlackboardVariable<ComboState>;
            var value = valueBlackboardVariable != null ? valueBlackboardVariable.Value : default(ComboState);

            Event?.Invoke(value);
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            ComboStateEventChannelEventHandler del = (value) =>
            {
                BlackboardVariable<ComboState> var0 = vars[0] as BlackboardVariable<ComboState>;
                if (var0 != null)
                    var0.Value = value;

                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as ComboStateEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as ComboStateEventChannelEventHandler;
        }
    }
}