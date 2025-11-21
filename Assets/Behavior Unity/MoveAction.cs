using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move", story: "[Agent] Move", category: "Action", id: "bc6d249568701656ed57306ce8edb818")]
public partial class MoveAction : CretureAction
{
    protected override Status OnUpdate()
    {
        /*
        if (WorldObject.Direction != Creature.WorldObject.Direct.None)
        {
            WorldObject.ChangeState(Creature.ObjectActionState.Move);
            return Status.Running;
        }
        */

        WorldObject.ChangeState(Creature.ObjectActionState.Move);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

