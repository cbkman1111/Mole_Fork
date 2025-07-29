using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Set Agent Input Active",
        story: "Set [Agent] input active: [Value]",
        category: "Action/Agent", id: "08e11e2ecd29b57bbf8c5b2b64af1540")]
    public partial class SetAgentInputActive : Action
    {
        [SerializeReference] public BlackboardVariable<AgentController> Agent;
        [SerializeReference] public BlackboardVariable<bool> Value;

        protected override Status OnStart()
        {
            if (Agent.Value == null)
            {
                LogFailure("No agent assigned.");
                return Status.Failure;
            }

            if (Value.Value)
            {
                Agent.Value.EnableInput();
            }
            else
            {
                Agent.Value.DisableInput();
            }

            return Status.Success;
        }
    }
}