using System;
using Unity.Behavior;
using Unity.Properties;
using Common.Utils;
using UnityEngine;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SerchSight", story: "SerchSight [Agent] See [Target]", category: "Action", id: "780099799002b93f70c0fdebd1b29ee7")]
public partial class SerchSightAction : CretureAction
{
    protected override Status OnStart()
    {
        base.OnStart();
        agent.ChangeAction(Creature.WorldObjectActionType.Idle);

        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        //GiantDebug.Log($"{agent.name}");
        if (Probs.Value == null)
            return Status.Failure;

        // Filter objects within a radius of 10 around the agent  
        Vector3 agentPosition = agent.transform.position;
        List<GameObject> nearbyObjects = Probs.Value.FindAll(obj =>
        {
            if (obj == null) 
                return false;

            float distance = Vector3.Distance(agentPosition, obj.transform.position);
            return distance <= 10f;
        });

        if (Target.Value == null)
        {
            var index = UnityEngine.Random.Range(0, nearbyObjects.Count);
            Target.Value = nearbyObjects[index];
        }

        // Log the count of nearby objects  
        // GiantDebug.Log($"Nearby objects count: {nearbyObjects.Count}");
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

