using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.Behavior.Demo
{
    /// <summary>
    /// Abstraction wrapper to control input of an AI Agent.
    /// This script will disable the assigned BehaviorGraphAgent to ensure it can be started
    /// at the right time.
    /// </summary>
    public class AIAgent : AgentController
    {
        [Header("AI Controller")]
        [SerializeField, Tooltip("The graph running the AI brain.")]
        private BehaviorGraphAgent m_AgentBrain;

        public override bool IsAI => true;
        public bool isInputReady { get; private set; } = false;

        private bool m_Running = false;
        private bool m_NeedRestart = false;

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(m_AgentBrain != null, $"[{Time.frameCount}] {this}: m_BehaviorAgent is required");
            if (m_AgentBrain.isActiveAndEnabled)
            {
                m_AgentBrain.End();
                m_AgentBrain.enabled = false;
                m_NeedRestart = true;
            }

            m_AgentBrain.BlackboardReference.SetVariableValue(k_StateChannelVariableName, StateChannel);
        }

        public override void EnableInput()
        {
            if (m_Running)
            {
                return;
            }

            m_Running = true;

            m_AgentBrain.enabled = true;

            if (m_NeedRestart)
            {
                m_NeedRestart = false;
                m_AgentBrain.Restart();
            }
        }

        public override void DisableInput()
        {
            if (m_Running == false)
            {
                return;
            }

            m_Running = false;
            m_AgentBrain.enabled = false;
        }

        private void OnValidate()
        {
            if (m_AgentBrain)
            {
                m_AgentBrain.enabled = false;
            }
        }
    }
}