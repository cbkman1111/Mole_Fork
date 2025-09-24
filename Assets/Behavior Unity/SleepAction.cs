using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sleep", story: "Sleep [House]", category: "Action", id: "6746bbdbf6665816c164b3fb78f5ae46")]
public partial class SleepAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> House;

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

