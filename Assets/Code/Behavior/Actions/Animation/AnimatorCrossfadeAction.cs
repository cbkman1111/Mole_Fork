using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Animator Crossfade",
        story: "Crossfade [Animator] state to [StateName] over [Duration] seconds",
        category: "Action/Animation",
        description: "Crossfade animator state to specified state.",
        id: "e23d2341832b0a2c78be96ff42eec93c")]
    public partial class AnimatorCrossfadeAction : Action
    {
        [SerializeReference] public BlackboardVariable<Animator> Animator;
        [SerializeReference] public BlackboardVariable<string> StateName;
        [SerializeReference] public BlackboardVariable<float> Duration = new(1f);
        [Tooltip("Set to false to make the node run until the next crossfade finishes.")]
        [SerializeReference] public BlackboardVariable<bool> RunInBackground = new(true);
        [SerializeReference] public BlackboardVariable<int> AnimatorLayerIndex = new(0);

        [CreateProperty]
        private float m_RemainingTime;

        protected override Status OnStart()
        {
            if (Animator.Value == null)
            {
                LogFailure("No Animator assigned.");
                return Status.Failure;
            }

            int stateHash = UnityEngine.Animator.StringToHash(StateName.Value);
            if (!Animator.Value.HasState(AnimatorLayerIndex, stateHash))
            {
                LogFailure($"No state '{StateName.Value}' found in {Animator.Value.gameObject.name}'s animator.");
                return Status.Failure;
            }

            Animator.Value.CrossFade(stateHash, Duration.Value);

            m_RemainingTime = Duration;
            return RunInBackground.Value ? Status.Success : Status.Running;
        }

        protected override Status OnUpdate()
        {
            // Time is inversely proportional to the animator's speed.
            m_RemainingTime -= Time.deltaTime / Animator.Value.speed;
            if (m_RemainingTime <= 0)
            {
                return Status.Success;
            }


            return Status.Running;
        }

        protected override void OnEnd()
        {
            m_RemainingTime = 0;
        }
    }
}