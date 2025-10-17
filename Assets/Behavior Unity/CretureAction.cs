using System.Collections.Generic;
using Creature;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

public partial class CretureAction: Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Cretures;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Probs;

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

        Cretures.Value = listCreture;
        Probs.Value = listProbs;
        return Status.Running;
    }
}
