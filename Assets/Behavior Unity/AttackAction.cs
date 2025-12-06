using System;
using Unity.Behavior;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "Attack [Agent] Attack [Target]", category: "Action", id: "bbf4bd44696ece6aafa5d50d2028c9a0")]
public partial class AttackAction : CreatureAction
{
    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

