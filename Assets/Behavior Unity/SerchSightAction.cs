using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using System.Collections.Generic;
using Creature;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SerchSight", story: "SerchSight [Agent] See [Target]", category: "Action", id: "780099799002b93f70c0fdebd1b29ee7")]
public partial class SerchSightAction : CretureAction
{
    private WorldObject LastTarget = null;

    protected override Status OnStart()
    {
        base.OnStart();

        WorldObject.ChangeState(ObjectActionState.Idle);
        return Status.Running;
    }

    /// <summary>
    /// TODO :  시야 내에 있는 오브젝트중에 우선순위가 높은 오브젝트를 타겟으로 설정.
    /// </summary>
    /// <returns></returns>
    protected override Status OnUpdate()
    {
        if (Probs.Value == null)
            return Status.Failure;

        Vector3 agentPosition = WorldObject.transform.position;
        List<GameObject> nearbyObjects = Probs.Value.FindAll(obj => {
            if (obj == null) 
                return false;

            float distance = Vector3.Distance(agentPosition, obj.transform.position);
            return distance <= 10f && LastTarget != obj;
        });

        for (int i = 0; i < nearbyObjects.Count; i++)
        {
            var index = i;//;UnityEngine.Random.Range(0, nearbyObjects.Count);
            var gobject = nearbyObjects[index];
            if (LastTarget == null)
            {
                LastTarget = gobject.GetComponent<WorldObject>();
                Target.Value = gobject;
                break;
            }
            else if (gobject != LastTarget.gameObject)
            {
                LastTarget = gobject.GetComponent<WorldObject>();
                Target.Value = gobject;
                break;
            }
        }
        

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

