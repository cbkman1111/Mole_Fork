using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Eat", story: "Eat [Food]", category: "Action", id: "02b53af373c0d67ef096853ecef1bb0b")]
public partial class EatAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Food;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

