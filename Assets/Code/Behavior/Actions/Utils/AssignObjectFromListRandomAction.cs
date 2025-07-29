using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Assign Object From List Random", 
        story: "Assign random from [ObjectList] to [Object]", 
        category: "Action/Utils", 
        id: "f4d5c8c73cdf52e1ba176a1210c4fbde")]
    public partial class AssignObjectFromListRandomAction : Action
    {
        [SerializeReference] public BlackboardVariable<List<GameObject>> ObjectList;
        [SerializeReference] public BlackboardVariable<GameObject> Object;

        protected override Status OnStart()
        {
            if (ObjectList.Value.Count == 0)
            {
                LogFailure($"Provided object list '{ObjectList.Name}' is empty.");
                return Status.Failure;
            }

            int count = ObjectList.Value.Count;
            Object.Value = ObjectList.Value[UnityEngine.Random.Range(0, count - 1)];

            return Status.Running;
        }
    }

}