using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Behavior.Demo
{
    public class PlayerAgentModule_Attack : PlayerAgentModule, ICharacterStateChannelModifier
    {
        public CharacterStateEventChannel StateChannel { get; set; }

        [SerializeField] private InputActionReference m_InputAction;
        
        private InputAction m_AttackAction;
        private bool m_CanAttack;
        private bool m_WantToAttack;

        public override bool ShouldUpdate()
        {
            return isActiveAndEnabled && m_WantToAttack;
        }

        public override void UpdateModule(float deltaTime)
        {
            StateChannel?.SendEventMessage(CharacterState.Attack);
            m_WantToAttack = false;
        }

        public override void EnableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
        {
            Debug.Assert(m_InputAction != null, "PlayerAgentModule_Attack: No input action reference provided.", this);
            m_AttackAction = activeActionMap.FindAction(m_InputAction.action.name);
            
            m_CanAttack = true;
            m_WantToAttack = false;

            m_AttackAction.performed += OnActionPerformed;
            StateChannel.Event += StateChannel_Event;
        }

        public override void DisableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
        {
            if (m_AttackAction != null)
            {
                m_AttackAction.performed -= OnActionPerformed;
            }
        }

        private void StateChannel_Event(CharacterState newState)
        {
            m_CanAttack = newState == CharacterState.Idle;
        }

        private void OnActionPerformed(InputAction.CallbackContext obj)
        {
            if (!m_CanAttack)
            {
                return;
            }

            m_WantToAttack = true;
        }
    }
}