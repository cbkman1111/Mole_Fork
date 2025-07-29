using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Release Object to Pool",
        story: "Release [Object] to [Pool]",
        category: "Action/Utils",
        id: "00a180f6175b91d6a12b8ece9b635b9a")]
    public partial class ReleaseObjectFromPoolAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Object;
        [SerializeReference] public BlackboardVariable<BehaviorAgentFactory> Pool;

        protected override Status OnStart()
        {
            if (Object.Value == null)
            {
                LogFailure("No object to release assigned.");
                return Status.Failure;
            }

            if (Pool.Value == null)
            {
                LogFailure("No pool assigned.");
                return Status.Failure;
            }

            Pool.Value.Release(Object.Value.GetComponent<BehaviorGraphAgent>());

            return Status.Success;
        }
    }
}
