using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Unity.Behavior.Demo
{
    public class PlayerAgentModule_Movement : PlayerAgentModule, ICharacterStateChannelModifier
    {
        [SerializeField] private InputActionReference m_InputAction;
        [SerializeField] private float m_MoveSpeed = 1f;
        private CharacterController m_CharacterController;
        private NavMeshAgent m_NavMeshAgent;
        private InputAction m_MoveAction;
        private Vector2 m_LastMoveInputValue;

        public CharacterStateEventChannel StateChannel { get; set; }
        private bool m_CanMove = true;

        public override void EnableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
        {
            m_NavMeshAgent = ControlledCharacter.GetComponent<NavMeshAgent>();
            m_MoveAction = activeActionMap.FindAction(m_InputAction.action.name);
            m_MoveAction.performed += OnMoveActionPerformed;
            m_MoveAction.canceled += OnMoveActionCanceled;
            m_CharacterController = ControlledCharacter.GetComponent<CharacterController>();
            m_CanMove = true;

            if (StateChannel)
            {
                StateChannel.Event += StateChannel_Event;
            }
        }

        public override void DisableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
        {
            if (m_MoveAction != null)
            {
                m_MoveAction.performed -= OnMoveActionPerformed;
                m_MoveAction.canceled -= OnMoveActionCanceled;
            }

            if (StateChannel)
            {
                StateChannel.Event -= StateChannel_Event;
            }
        }

        public override void UpdateModule(float deltaTime)
        {
            if (m_CanMove == false || m_LastMoveInputValue == Vector2.zero)
            {
                return;
            }

            Vector3 dir = new Vector3(m_LastMoveInputValue.x, 0, m_LastMoveInputValue.y);
            if (m_NavMeshAgent)
            {
                m_NavMeshAgent.Move(dir * m_MoveSpeed * deltaTime);
            }
            else if (m_CharacterController)
            {
                m_CharacterController.Move(dir * m_MoveSpeed * deltaTime);
            }
        }

        private void OnMoveActionPerformed(InputAction.CallbackContext obj)
        {
            m_LastMoveInputValue = obj.ReadValue<Vector2>();
        }

        private void OnMoveActionCanceled(InputAction.CallbackContext obj)
        {
            m_LastMoveInputValue = Vector2.zero;
        }

        private void StateChannel_Event(CharacterState newState)
        {
            m_CanMove = newState == CharacterState.Idle;
        }
    }
}