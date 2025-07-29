using UnityEngine;

namespace Unity.Behavior.Demo
{
    public class GameStateEvent : MonoBehaviour
    {
        [SerializeField] private GameStateEventChannel m_GameStateEventChannel;
        [SerializeField] private GameState m_TargetState;

        [ContextMenu("Send EventChannel Message")]
        public void SendChannelEvent()
        {
            m_GameStateEventChannel.SendEventMessage(m_TargetState);
        }
    }
}