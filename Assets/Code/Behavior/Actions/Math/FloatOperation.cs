using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Float Operation",
        story: "[Result] = [LHS] [Operator] [RHS]",
        category: "Action/Math",
        id: "70f0230ceb4a353e08ec5b56e438045a")]
    public partial class FloatOperation : Action
    {
        public enum OperatorType
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        [SerializeReference]
        public BlackboardVariable<float> Result;

        [SerializeReference]
        public BlackboardVariable<float> LHS;

        [SerializeReference]
        public BlackboardVariable<OperatorType> Operator;

        [SerializeReference]
        public BlackboardVariable<float> RHS;

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
            };

            return Status.Success;
        }
    }
}