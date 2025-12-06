using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Creature;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SerchSight", story: "SerchSight [Agent] See [Target]", category: "Action", id: "780099799002b93f70c0fdebd1b29ee7")]
public partial class SerchSightAction : CreatureAction
{
    // [설정] 감지 범위 (인스펙터에서 수정 가능하도록 Blackboard 변수 사용 권장)
    [SerializeReference] public BlackboardVariable<float> DetectRange = new BlackboardVariable<float>(10f);

    // 이전 타겟 기억 (왔다갔다 방지용, 필요 없다면 제거 가능)
    private GameObject _lastFoundTarget = null;

    protected override Status OnStart()
    {
        base.OnStart();

        return Status.Running;
    }

    /// <summary>
    /// TODO :  시야 내에 있는 오브젝트중에 우선순위가 높은 오브젝트를 타겟으로 설정.
    /// </summary>
    /// <returns></returns>
    protected override Status OnUpdate()
    {
        // 1. 데이터 유효성 검사
        if (Props == null || Props.Value == null)
            return Status.Failure;

        if(Props.Value.Count == 0)
            return Status.Failure;

        Vector3 myPos = WorldObject.transform.position;
        float rangeSqr = DetectRange.Value * DetectRange.Value; // 거리 제곱 (최적화)

        GameObject bestTarget = null;
        float minDistanceSqr = float.MaxValue;

        // 2. 리스트 순회 (GC Alloc 없음)
        foreach (var obj in Props.Value)
        {
            if (obj == null || !obj.activeInHierarchy) 
                continue;

            // 이전에 찾았던 타겟은 일단 후순위로 미루거나 제외할 수 있음
            if (obj == _lastFoundTarget) 
                continue;

            // 거리 계산 (제곱 비교)
            Vector3 diff = obj.transform.position - myPos;
            float distSqr = diff.sqrMagnitude;

            // 범위 안에 있고, 지금까지 찾은 것보다 더 가까우면 갱신
            if (distSqr <= rangeSqr && distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                bestTarget = obj;
            }
        }

        // 3. 결과 처리
        if (bestTarget != null)
        {
            _lastFoundTarget = bestTarget;
            Target.Value = bestTarget;

            // [옵션] 찾았으면 즉시 상태 변경 등을 수행할 수도 있음
            // WorldObject.SetActionState(ObjectActionState.Chase); 

            return Status.Success; // 찾았음!
        }

        // 아무것도 못 찾았으면 Failure를 리턴해야 Behavior Graph가 다른 행동(예: 순찰)을 시도함
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

