using UnityEngine.InputSystem;

namespace Unity.Behavior.Demo
{
    public abstract class PlayerAgentModule : AgentControllerModule<PlayerAgent>
    {
        public PlayerInput PlayerInput { get; private set; }

        public abstract void EnableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap);

        public abstract void DisableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap);

        public override void InitModule(AgentController controller)
        {
            base.InitModule(controller);

            PlayerInput = ModuleOwner.playerInput;
        }
    }
}