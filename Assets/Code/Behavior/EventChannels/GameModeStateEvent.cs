using UnityEngine;

namespace Unity.Behavior.Demo
{
    public class GameModeStateEvent : MonoBehaviour
    {
        [SerializeField] private GameModeStateEventChannel m_GameflowEventChannel;
        [SerializeField] private GameModeState m_TargetState;

        [ContextMenu("Send EventChannel Message")]
        public void SendChannelEvent()
        {
            m_GameflowEventChannel.SendEventMessage(m_TargetState);
        }
    }
}