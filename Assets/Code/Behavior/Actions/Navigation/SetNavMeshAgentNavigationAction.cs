using System;
using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Set Agent Navigation",
        story: "[Action] [NavMeshAgent] navigation",
        category: "Action/Navigation",
        description: "Pauses/Resumes NavMeshAgent navigation.",
        id: "7f6f31f1009a2d3de04b559ca3e3f2af")]
    public partial class SetNavMeshAgentNavigationAction : Action
    {
        [BlackboardEnum]
        public enum ActionType
        {
            Pause,
            Resume,
        }

        [SerializeReference] public BlackboardVariable<NavMeshAgent> NavMeshAgent;
        [SerializeReference] public BlackboardVariable<ActionType> Action;

        protected override Status OnStart()
        {
            if (NavMeshAgent.Value == null)
            {
                LogFailure("No NavMeshAgent assigned.");
                return Status.Failure;
            }

            NavMeshAgent.Value.isStopped = Action.Value == ActionType.Pause;
            return Status.Success;
        }
    }
}