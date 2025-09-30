using System;
using Unity.Behavior;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Creature;
using UnityEngine;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SerchRange", 
    story: "SerchRange [Agent] Check Skill Range [Enemy]", 
    category: "Action", 
    id: "bf1918c16e89190eb534a59b4208c9f1")]
public partial class SerchRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<WorldObject> Agent;
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

