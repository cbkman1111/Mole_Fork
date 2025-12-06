using Creature;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

public partial class CreatureAction: Action
{
    // [수정 2] 입력/출력 변수 정의
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<List<GameObject>> Creatures;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Props;

    // 캐싱 변수
    protected BlackboardReference BlackBoard = null;
    protected WorldObject WorldObject = null;
    protected NavMeshAgent NavAgent = null;

    protected override Status OnStart()
    {
        GameObject go = Agent.Value;
        if (go == null)
            return Status.Failure;

        WorldObject = go.GetComponent<WorldObject>();
        if (WorldObject == null)
            return Status.Failure;

        BlackBoard = WorldObject.BlackboardReference;
        if (BlackBoard == null)
            return Status.Failure;

        NavAgent = WorldObject.GetComponent<NavMeshAgent>();
        if (BlackBoard == null)
            return Status.Failure;

        BlackBoard.GetVariableValue("Creatures", out List<GameObject> listCreture);
        BlackBoard.GetVariableValue("Probs", out List<GameObject> listProbs);

        Creatures.Value = listCreture;
        Props.Value = listProbs;
        return Status.Running;
    }
    protected void LogFailure(string msg)
    {
        // Debug.LogWarning($"[CreatureAction] Failure: {msg}");
    }
}
