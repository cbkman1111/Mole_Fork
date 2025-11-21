using Creature;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InteractionAction", story: "[Agent] Interaction [Type] [Target]", category: "Action", id: "3e93b00caf9249f77d4fd56a92a89ad7")]
public partial class InteractionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<InteractionType> Type;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if(Agent.Value == null)
            return Status.Failure;
        
        if (Target.Value == null)
            return Status.Failure;

        var agent = Agent.Value.gameObject.GetComponent<WorldObject>();
        var target = Target.Value.gameObject.GetComponent<WorldObject>();
        target.OnReciveInteraction(Type);

        return Status.Running;
    }
}

