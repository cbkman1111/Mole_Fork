using System;
using Unity.Behavior;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Idle", story: "[Agent]  Idle", category: "Action", id: "ec172a0f6d4a1c4f27e9a8cd53ab937d")]
public partial class IdleAction : CreatureAction
{
    protected override Status OnStart()
    {
        base.OnStart();
        
        //WorldObject.ChangeState(Creature.ObjectActionState.Idle);
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

