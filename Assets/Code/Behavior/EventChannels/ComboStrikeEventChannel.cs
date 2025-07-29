using System;
using Unity.Properties;

namespace Unity.Behavior.Demo
{
#if UNITY_EDITOR
    [UnityEngine.CreateAssetMenu(menuName = "Behavior/Event Channels/Combo Strike Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(
        name: "Combo Strike Event Channel", 
        message: "Strike is requested", 
        category: "Events", 
        id: "94c4e9ade712bd63ac5b9997dad94ed5")]
    public partial class ComboStrikeEventChannel : EventChannelBase
    {
        public delegate void ComboStrikeEventChannelEventHandler();
        public event ComboStrikeEventChannelEventHandler Event;

        public void SendEventMessage()
        {
            Event?.Invoke();
        }

        public override void SendEventMessage(BlackboardVariable[] messageData)
        {
            Event?.Invoke();
        }

        public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
        {
            ComboStrikeEventChannelEventHandler del = () =>
            {
                callback();
            };
            return del;
        }

        public override void RegisterListener(Delegate del)
        {
            Event += del as ComboStrikeEventChannelEventHandler;
        }

        public override void UnregisterListener(Delegate del)
        {
            Event -= del as ComboStrikeEventChannelEventHandler;
        }
    }
}