using System;
using Creature;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Agent] Chase [Target]", category: "Action", id: "b1f0179be5752e9a4bd0dfe92ecdbfb0")]
public partial class ChaseAction : CreatureAction
{
    // [설정] 경로 갱신 주기 (초)
    private const float PathUpdateInterval = 0.2f;
    private float _timeSinceLastPathUpdate;

    protected override Status OnStart()
    {
        base.OnStart();

        // 안전장치
        if (NavAgent == null || Target.Value == null) 
            return Status.Failure;

        NavAgent.isStopped = false;
        NavAgent.updateRotation = true; // 회전도 맡김

        // 시작하자마자 이동하도록 초기화
        _timeSinceLastPathUpdate = PathUpdateInterval;

        //WorldObject.ChangeState(ObjectActionState.Chase);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (NavAgent == null) 
            return Status.Failure;
        if (Target.Value == null) 
            return Status.Failure; // 타겟이 사라지면 보통 실패 처리
        if (!NavAgent.pathPending)
        {
            if (NavAgent.remainingDistance <= NavAgent.stoppingDistance)
            {
                if (!NavAgent.hasPath || NavAgent.velocity.sqrMagnitude == 0f)
                {
                    // 도착함
                    return Status.Success;
                }
            }
        }

        // 2. 경로 갱신 (스로틀링 적용)
        _timeSinceLastPathUpdate += Time.deltaTime;
        if (_timeSinceLastPathUpdate >= PathUpdateInterval)
        {
            _timeSinceLastPathUpdate = 0f;

            // SamplePosition 제거 -> SetDestination이 내부적으로 처리함
            NavAgent.SetDestination(Target.Value.transform.position);
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        // NavAgent가 파괴되지 않았고, 게임오브젝트가 활성화 상태일 때만 정지 명령
        if (NavAgent != null && NavAgent.isActiveAndEnabled && NavAgent.isOnNavMesh)
        {
            NavAgent.isStopped = true;
            NavAgent.ResetPath();
        }
    }
}

