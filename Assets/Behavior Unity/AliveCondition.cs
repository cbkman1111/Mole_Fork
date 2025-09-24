using System;
using Creature;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "Alive", 
    description: "Checks if the entity is alive.",
    story: "[Agent] is Alive", 
    category: "Conditions", 
    id: "dcb021c204446d837cc457b500718540")]
public partial class AliveCondition : Condition
{
    [Tooltip("The GameObject to show the text over.")]
    [SerializeReference] public BlackboardVariable<WorldObject> Agent;

    public override bool IsTrue()
    {
        var obj = Agent.Value.GetComponent<WorldObject>();
        if (obj.Stat.Health == 0)
            return false;
        /*
        Unity.Behavior.Blackboard blackboard = GetBlackboard();
        if (blackboard == null)
            return false;
        */

        // Example usage of blackboard (if needed)
        // var self = blackboard.Get<WorldObject>("Self");

        return true;
    }



    public override void OnStart()
    {

    }


    public override void OnEnd()
    {
    }
}
