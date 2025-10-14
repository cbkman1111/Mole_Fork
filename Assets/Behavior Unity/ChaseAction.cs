using System;
using System.Numerics;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Agent] Chase [Target]", category: "Action", id: "b1f0179be5752e9a4bd0dfe92ecdbfb0")]
public partial class ChaseAction : CretureAction
{
    private NavMeshAgent navAgent = null;

    protected override Status OnStart()
    {
        base.OnStart();

        agent.ChangeAction(Creature.WorldObjectActionType.Chase);
        navAgent = agent.GetComponent<NavMeshAgent>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (navAgent == null)
            return Status.Failure;

        if (Target.Value == null)
            return Status.Success;

        //agent.Stat.GetStat(Creature.Stat.StatType.Weight);
        var positionTarget = Target.Value.transform.position;
        var positionAgent = agent.transform.position;

        if (navAgent.pathPending == false)
        {
            const float stoppingDistance = 4.0f;
            if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
            {
                var diff = navAgent.transform.position - hit.position;
                if(UnityEngine.Vector3.SqrMagnitude(diff) < stoppingDistance * stoppingDistance)
                {
                    Target.Value = null;
                    return Status.Success;
                }

                navAgent.SetDestination(Target.Value.transform.position);
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

