using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Random Position In Circle",
        story: "[Value] = Random point around [Target]",
        description: "Assigns a random point around the target to Value.",
        category: "Action/Math",
        id: "4438f519474be31295544ec53be8bd60")]
    public partial class RandomPositionInCircleAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> Value;
        [SerializeReference] public BlackboardVariable<Transform> Target;
        [SerializeReference] public BlackboardVariable<float> MinimumRadius = new(1);
        [SerializeReference] public BlackboardVariable<float> MaximumRadius = new(3);

        protected override Status OnStart()
        {
            if (Target.Value == null)
            {
                LogFailure("No target assigned.");
                return Status.Failure;
            }

            var randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            var randomRadius = UnityEngine.Random.Range(MinimumRadius.Value, MaximumRadius.Value);

            // Calculate the position using polar coordinates
            var x = randomRadius * Mathf.Cos(randomAngle);
            var z = randomRadius * Mathf.Sin(randomAngle);
            Value.Value = Target.Value.position + new Vector3(x, 0, z);

            return Status.Success;
        }
    }
}
