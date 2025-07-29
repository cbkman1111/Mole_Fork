using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Input Performed",
        story: "On Input [InputAction] [Action]",
        category: "Flow/Conditional",
        description: "Responds to project-wide input action event.",
        id: "bcebc61cbe4788484f2ec4a94f724180")]
    public partial class InputEventModifier : Modifier
    {
        [BlackboardEnum]
        public enum InputActionType
        {
            Performed,
            Cancelled
        }

        /// <summary>
        /// The input action reference to listen for. The action must be part of the InputSystem Project-wide Action asset
        /// (Project Settings > Input System Package > Project-wide Action).
        /// </summary>
        [Tooltip("The input action reference to listen for." +
            "\nThe action must be part of the InputSystem Project-wide Action asset " +
            "(Project Settings > Input System Package > Project-wide Action).")]
        [SerializeReference] public BlackboardVariable<InputActionReference> InputAction;

        /// <summary>
        /// Should the modifier only trigger once per call?
        /// Set to false will trigger everytime the input is performed after it's children nodes have completed.
        /// </summary>
        [Tooltip("Should the modifier only trigger once per call?" +
            "\nSet to false will trigger everytime the input is performed after it's children nodes have completed.")]
        [SerializeReference] public BlackboardVariable<bool> TriggerOnlyOnce;

        /// <summary>
        /// The type of action performed.
        /// </summary>
        [SerializeReference] public BlackboardVariable<InputActionType> Action;

        private InputAction m_Action;

        protected override Status OnStart()
        {
            if (Child == null)
            {
                return Status.Success;
            }

            if (InputAction.Value == null)
            {
                LogFailure("No valid input action reference set.");
                return Status.Failure;
            }

            m_Action = InputSystem.actions.FindAction(InputAction.Value.name);

            if (m_Action == null)
            {
                LogFailure($"Failed to find action {InputAction.Value.name} as part of the InputSystem Project-wide Action asset " +
                    $"(Project Settings > Input System Package > Project-wide Action).");
                return Status.Failure;
            }

            BindEvent();

            return Status.Waiting;
        }

        protected override Status OnUpdate()
        {
            if (TriggerOnlyOnce.Value)
            {
                Status status = Child.CurrentStatus;

                if (status is Status.Uninitialized)
                {
                    Status childStatus = StartNode(Child);
                    if (childStatus is Status.Success or Status.Failure)
                    {
                        return childStatus;
                    }
                }

                if (status == Status.Failure || status == Status.Success)
                {
                    return status;
                }
            }
            else
            {
                Status status = Child.CurrentStatus;

                if (status is Status.Uninitialized)
                {
                    var childStatus = StartNode(Child);
                    if (childStatus is Status.Success or Status.Failure)
                    {
                        ResetStatus();
                        return Status.Waiting;
                    }

                    return childStatus;
                }

                if (status == Status.Failure || status == Status.Success)
                {
                    ResetStatus();
                    return Status.Waiting;
                }

                return status;
            }

            return Status.Waiting;
        }

        private void OnActionPerformed(InputAction.CallbackContext obj)
        {
            AwakeNode(this);
        }

        protected override void OnDeserialize()
        {
            // OnDeserialize would only be called if the node status was running/waiting.
            // so we know we need to rebind the event.

            m_Action = InputSystem.actions.FindAction(InputAction.Value.name);
            if (m_Action != null)
            {
                BindEvent();
            }
        }

        private void BindEvent()
        {
            switch (Action.Value)
            {
                case InputActionType.Performed:
                    m_Action.performed += OnActionPerformed;
                    break;
                case InputActionType.Cancelled:
                    m_Action.canceled += OnActionPerformed;
                    break;
            }
        }
    }
}