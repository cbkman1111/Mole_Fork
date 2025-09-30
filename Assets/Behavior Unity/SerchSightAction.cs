using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Creature;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SerchSight", story: "SerchSight [Agent] See [Enemy]", category: "Action", id: "780099799002b93f70c0fdebd1b29ee7")]
public partial class SerchSightAction : Action
{
    [Tooltip("The GameObject Agent Self.")]
    [SerializeReference] public BlackboardVariable<WorldObject> Agent;

    [Tooltip("The GameObjects to see enemys.")]
    [SerializeReference] public BlackboardVariable<List<GameObject>> Enemy;

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

