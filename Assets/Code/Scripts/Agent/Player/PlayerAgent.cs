using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Behavior.Demo
{
    public class PlayerAgent : AgentController<PlayerAgentModule>
    {
        [Header("Player Controller")]
        [SerializeField]
        protected PlayerInput m_PlayerInput;

        [SerializeField, Tooltip("Action map used by this controller to get bindings from.")]
        private string m_ActionMapName = "Player";

        public override bool IsAI => false;
        public PlayerInput playerInput => m_PlayerInput;
        public bool isInputReady { get; private set; } = false;

        private InputActionMap m_ActionMap;

        private bool m_Running = false;

        protected override void Awake()
        {
            base.Awake();

            CapturePlayerInputAndSetBehaviour();
        }

        public virtual void MountPlayerInput(PlayerInput player, bool enableInput = true)
        {
            m_PlayerInput = player;

            if (enableInput)
            {
                EnableInput();
            }
        }

        public virtual void UnMountPlayerInput()
        {
            DisableInput();
            m_PlayerInput = null;
        }

        public override void EnableInput()
        {
            if (m_Running)
            {
                return;
            }

            m_Running = true;

            Debug.Assert(m_PlayerInput != null, $"[{Time.frameCount}] {this}: PlayerInput is required");

            m_PlayerInput.ActivateInput();
            m_PlayerInput.SwitchCurrentActionMap(m_ActionMapName);
            m_ActionMap = m_PlayerInput.actions.FindActionMap(m_ActionMapName);

            isInputReady = true;

            foreach (var extension in m_Modules)
            {
                if (!extension.enabled)
                {
                    continue;
                }

                extension.EnableModuleInput(playerInput, m_ActionMap);
            }
        }

        public override void DisableInput()
        {
            if (m_Running == false)
            {
                return;
            }

            m_Running = false;

            m_PlayerInput.DeactivateInput();
            m_ActionMap = null;
            isInputReady = false;

            foreach (var extension in m_Modules)
            {
                if (!extension.enabled)
                {
                    continue;
                }

                extension.DisableModuleInput(playerInput, m_ActionMap);
            }
        }

        protected override void OnDestroy()
        {
            UnMountPlayerInput();
        }

        private void Update()
        {
            UpdateController(Time.deltaTime);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            CapturePlayerInputAndSetBehaviour();
        }

        private void CapturePlayerInputAndSetBehaviour()
        {
            if (m_PlayerInput == null)
            {
                m_PlayerInput = GetComponentInParent<PlayerInput>();
            }

            if (m_PlayerInput)
            {
                m_PlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            }
        }
    }
}