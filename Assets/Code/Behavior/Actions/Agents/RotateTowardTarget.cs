using System;
using UnityEngine;
using Unity.Properties;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
    name: "Rotate Toward Target",
    story: "Rotate [Agent] toward [Target] over [Duration] seconds",
    category: "Action",
    id: "935ad17b5396de1ba2f7be96f61534af")]
    public partial class RotateTowardTarget : Action
    {
        [SerializeReference] public BlackboardVariable<Transform> Agent;
        [SerializeReference] public BlackboardVariable<Transform> Target;
        [SerializeReference] public BlackboardVariable<float> Duration = new(1f);
        [Tooltip("Set to true to track the target position instead of using only it's value when the node start.")]
        [SerializeReference] public BlackboardVariable<bool> Track = new(false);
        [Tooltip("If the value set is less than Duration, the node returns a success when this duration is reached.")]
        [SerializeReference] public BlackboardVariable<float> Timeout = new(1f);

        [CreateProperty] private float m_CurrentDuration;
        [CreateProperty] private Quaternion m_OriginRotation;
        [CreateProperty] private Quaternion m_TargetRotation;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                LogFailure("No Agent or Target provided.");
                return Status.Failure;
            }

            if (Duration.Value <= 0.0f)
            {
                Vector3 targetPosition = Target.Value.position;
                targetPosition.y = Agent.Value.position.y;
                Agent.Value.LookAt(targetPosition, Agent.Value.up);
                return Status.Success;
            }

            m_OriginRotation = Agent.Value.rotation;
            if (Track.Value == false)
            {
                var dir = (Target.Value.position - Agent.Value.position);
                dir.y = 0;
                m_TargetRotation = Quaternion.LookRotation(dir.normalized);
            }

            m_CurrentDuration = 0;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            bool track = Track.Value;
            if (Agent.Value == null || (track && Target.Value == null))
            {
                return Status.Failure;
            }

            m_CurrentDuration += Time.deltaTime;
            if (m_CurrentDuration >= Timeout.Value)
            {
                return Status.Success;
            }

            if (track)
            {
                var dir = (Target.Value.position - Agent.Value.position);
                dir.y = 0;
                m_TargetRotation = Quaternion.LookRotation(dir.normalized);
            }

            float fraction = m_CurrentDuration / Duration.Value;
            if (fraction < 1.0f)
            {
                Agent.Value.rotation = Quaternion.Slerp(m_OriginRotation, m_TargetRotation, fraction);
                return Status.Running;
            }
            else
            {
                Agent.Value.rotation = m_TargetRotation;
                return Status.Success;
            }
        }
    }
}
