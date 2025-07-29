using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Integer Operation",
        story: "[Result] = [LHS] [Operator] [RHS]",
        category: "Action/Math",
        id: "70f0230ceb4a353e08ec5b56e438045b")]
    public partial class IntegerOperation : Action
    {
        public enum OperatorType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            IncrementAndRepeat
        }

        [SerializeReference]
        public BlackboardVariable<int> Result;

        [SerializeReference]
        public BlackboardVariable<int> LHS;

        [SerializeReference]
        public BlackboardVariable<OperatorType> Operator;

        [SerializeReference]
        public BlackboardVariable<int> RHS;

        protected override Status OnStart()
        {
            switch (Operator.Value)
            {
                case OperatorType.Add:
                    Result.Value = LHS.Value + RHS.Value;
                    break;

                case OperatorType.Subtract:
                    Result.Value = LHS.Value - RHS.Value;
                    break;

                case OperatorType.Multiply:
                    Result.Value = LHS.Value * RHS.Value;
                    break;

                case OperatorType.Divide:
                    Result.Value = LHS.Value / RHS.Value;
                    break;

                case OperatorType.IncrementAndRepeat:
                    Result.Value = (int)Mathf.Repeat(LHS.Value + 1, RHS.Value);
                    break;
            };

            return Status.Success;
        }
    }
}