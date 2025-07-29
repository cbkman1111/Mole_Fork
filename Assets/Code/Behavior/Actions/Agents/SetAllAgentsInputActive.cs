using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Set All Agents Input Active",
        story: "Set all agents input active: [Value]",
        category: "Action/Agent", id: "862b8d52ce082dcd6a4bf57509839ac4")]
    public partial class SetAllAgentsInputActive : Action
    {
        [SerializeReference]
        public BlackboardVariable<bool> Value;

        protected override Status OnStart()
        {
            var agents = GameObject.FindObjectsByType<AgentController>(FindObjectsSortMode.None);

            if (agents == null || agents.Length == 0)
            {
                Debug.LogWarning($"No {typeof(AgentController).Name} found in the scene.");
                return Status.Failure;
            }

            if (Value.Value)
            {
                foreach (var agent in agents)
                {
                    agent.EnableInput();   
                }
            }
            else
            {
                foreach (var agent in agents)
                {
                    agent.DisableInput();
                }
            }

            return Status.Success;
        }
    }
}