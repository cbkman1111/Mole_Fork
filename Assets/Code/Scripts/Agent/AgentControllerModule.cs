using UnityEngine;

namespace Unity.Behavior.Demo
{
    public abstract class AgentControllerModule : MonoBehaviour
    {
        public GameObject ControlledCharacter { get; private set; }

        public virtual void InitModule(AgentController controller)
        {
            ControlledCharacter = controller.ControlledCharacter;
        }

        public virtual void UpdateModule(float deltaTime)
        {
        }

        public virtual bool ShouldUpdate()
        {
            return isActiveAndEnabled;
        }
    }

    public abstract class AgentControllerModule<T> : AgentControllerModule
        where T : AgentController
    {
        public T ModuleOwner { get; private set; }

        public override void InitModule(AgentController controller)
        {
            ModuleOwner = controller as T;
            base.InitModule(controller);
        }
    }
}