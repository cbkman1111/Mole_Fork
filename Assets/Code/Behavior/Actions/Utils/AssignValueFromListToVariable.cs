using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Assign Value From List To Variable",
        story: "Assign [list] value at index [i] to [variable]",
        category: "Action/Utils", 
        id: "302ea7a2145b099cece5024f44a358e1")]
    public partial class AssignValueFromListToVariable : Action
    {
        [SerializeReference] public BlackboardVariable<List<string>> List;
        [SerializeReference] public BlackboardVariable<int> I;
        [SerializeReference] public BlackboardVariable<string> Variable;

        protected override Status OnStart()
        {
            if (I.Value >= List.Value.Count)
            {
                LogFailure($"Index {I.Value} is out of range {List.Value.Count}");
                return Status.Failure;
            }

            Variable.Value = List.Value[I];

            return Status.Success;
        }
    }
}