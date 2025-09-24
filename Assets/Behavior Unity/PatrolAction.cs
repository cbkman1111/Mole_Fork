using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "Patrol [Point]", category: "Action", id: "473a9b37107f67f90996b4a33040f773")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Point;

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

