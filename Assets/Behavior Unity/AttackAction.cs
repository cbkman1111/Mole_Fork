using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Creature;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using Org.BouncyCastle.Bcpg.Sig;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "Attack [Agent] Attack [Target]", category: "Action", id: "bbf4bd44696ece6aafa5d50d2028c9a0")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private BlackboardReference blackBoard = null;
    private WorldObject agent = null;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null)
            return Status.Failure;

        GameObject go = Agent.Value;
        if (go == null)
            return Status.Failure;

        agent = go.GetComponent<WorldObject>();
        if(agent == null)
        return Status.Failure;  

        blackBoard = agent.BlackboardReference;
        if (blackBoard == null)
            return Status.Failure;

        //blackBoard.GetVariable("Creatures", out var creatures);
        List<GameObject> creatures = null;
        if (blackBoard.GetVariable<List<GameObject>>("Creatures", out var creaturesVar) == true)
        {
            creatures = creaturesVar.Value;
            foreach (var creture in creatures)
            {
                if (creture == null) 
                    continue;

                // WorldObject 컴포넌트 얻기
                var worldObj = creture.GetComponent<WorldObject>();
                if (worldObj != null)
                {
                    if (worldObj == agent)
                    {
                        Debug.Log($"Mine : {worldObj.name}");
                    }
                    else
                    {
                        //Debug.Log($"Other : {worldObj.name}");
                    }
                }
            }
        }
        else
        { 
        }

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

