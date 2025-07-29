using System;
using System.Security.Cryptography;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Spawn Object From Pool ",
        story: "Spawn From [Pool]",
        category: "Action/Utils",
        id: "283af28cce286366569d891d85c8a04a")]
    public partial class SpawnObjectFromPoolAction : Action
    {
        [SerializeReference] public BlackboardVariable<BehaviorAgentFactory> Pool;
        [Tooltip("[Out Value] Assigned the spawn object to the assigned field.")]
        [SerializeReference] public BlackboardVariable<GameObject> Object;
        [SerializeReference] public BlackboardVariable<Vector3> Location = new (Vector3.zero);

        protected override Status OnStart()
        {
            if (Pool.Value == null)
            {
                LogFailure("No pool assigned");
            }

            Object.Value = Pool.Value.Get().gameObject;
            Object.Value.transform.position = Location;

            return Status.Success;
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}
