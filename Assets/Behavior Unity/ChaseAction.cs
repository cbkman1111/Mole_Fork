using System;
using Creature;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Agent] Chase [Target]", category: "Action", id: "b1f0179be5752e9a4bd0dfe92ecdbfb0")]
public partial class ChaseAction : CretureAction
{

    protected override Status OnStart()
    {
        base.OnStart();

        NavAgent.isStopped = false;
        WorldObject.ChangeState(ObjectActionState.Chase);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (NavAgent == null)
            return Status.Failure;

        if (Target.Value == null)
            return Status.Success;

        //agent.Stat.GetStat(Creature.Stat.StatType.Weight);
        var positionTarget = Target.Value.transform.position;
        var positionAgent = WorldObject.transform.position;

        if (NavAgent.pathPending == false)
        {
            const float stoppingDistance = 0.1f;
            if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
            {
                var diff = NavAgent.transform.position - hit.position;
                if(UnityEngine.Vector3.SqrMagnitude(diff) < stoppingDistance * stoppingDistance)
                {
                    //Target.Value = null;
                    NavAgent.isStopped = true;
                    return Status.Success;
                }

                NavAgent.SetDestination(Target.Value.transform.position);
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

