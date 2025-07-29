using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Set Variable To List Size", 
        story: "Set [Variable] to [List] size",
        category: "Action/Utils", 
        id: "302ea7a2145b099cece5024f44a358ew")]
    public partial class SetVariableToListSizeAction : Action
    {
        [SerializeReference] public BlackboardVariable<int> Variable;
        [SerializeReference] public BlackboardVariable<List<string>> List;

        protected override Status OnStart()
        {
            if (List.Value.Count == 0)
            {
                LogFailure("List is empty.");
            }

            Variable.Value = List.Value == null ? 0 : List.Value.Count;
            return Status.Success;
        }
    }
}