using System;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, Properties.GeneratePropertyBag]
    [Condition(
        name: "Null Check",
        story: "[Variable] is null",
        category: "Variable Conditions",
        description: "Null check a reference type. Return false if a value type is provided.",
        id: "62675c06b1462f2a1f8291f1e3255339")]
    public partial class NullCheckCondition : Condition
    {
        [SerializeReference] public BlackboardVariable Variable;

        public override bool IsTrue()
        {
            if (Variable.Type.IsValueType)
            {
                return false;
            }

            return ReferenceEquals(Variable.ObjectValue, null);
        }
    }
}