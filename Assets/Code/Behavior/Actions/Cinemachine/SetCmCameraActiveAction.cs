using System;
using UnityEngine;
using Unity.Properties;
using Unity.Cinemachine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Set Cm Camera Active",
        story: "Set [CmCamera] active: [Value]",
        category: "Action/Cinemachine",
        id: "1f6f32f1009a2d3de04b559ca3e3a2af")]
    public partial class SetCmCameraActiveAction : Action
    {
        [SerializeReference] public BlackboardVariable<CinemachineCamera> CmCamera;
        [SerializeReference] public BlackboardVariable<bool> Value;

        protected override Status OnStart()
        {
            if (CmCamera.Value == null)
            {
                LogFailure("No Cinemachine Camera assigned.");
                return Status.Failure;
            }

            CmCamera.Value.enabled = Value.Value;
            return Status.Success;
        }
    }
}