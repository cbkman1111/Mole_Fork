using System;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable]
    [Condition(
            name: "Check FoV",
            category: "Conditions",
            story: "[Origin] has [Target] in view below [Angle] degrees",
            id: "17f18739e880f2880a0c8b3df7c50061")]
    public class CheckTargetInFieldOfViewCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<Transform> Origin;
        [SerializeReference] public BlackboardVariable<Transform> Target;
        [SerializeReference] public BlackboardVariable<float> Angle = new(90f);

        public override bool IsTrue()
        {
            if (Origin.Value == null || Target.Value == null)
            {
                return false;
            }

            float distance = Vector3.Distance(Origin.Value.position, Target.Value.position);
            return IsLookingAtTarget(Origin, Target.Value.position, Angle)
                && HasTargetInView(Origin.Value.position, Target);
        }

        private static bool IsLookingAtTarget(Transform origin, Vector3 targetPosition, float angle)
        {
            var forward = origin.forward;
            var toTarget = targetPosition - origin.position;
            var currentAngle = Vector3.Angle(forward, toTarget);
            return currentAngle < angle * 0.5f; // andlge is divided by 2 as it is angles in both direction.
        }

        private static bool HasTargetInView(Vector3 origin, Transform target)
        {
            if (Physics.Linecast(origin, target.position, out var t))
            {
                return t.collider.gameObject == target.gameObject;
            }

            return false;
        }
    }
}