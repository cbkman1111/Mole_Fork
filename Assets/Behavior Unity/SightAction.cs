using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Sight", 
    story: "TargetInSight", 
    category: "Action", 
    id: "b826a718c153d5d809ec01b63fa9368f")]
public partial class SightAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

