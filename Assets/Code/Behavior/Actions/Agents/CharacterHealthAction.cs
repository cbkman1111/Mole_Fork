using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Character Health",
        story: "[Action] [Target] of [Amount] HP",
        description: "Heals or damages the target character by the specified amount of HP.",
        category: "Action/Gameplay",
        id: "1cfcadc343b583e6258c162ed0c16449")]
    public partial class CharacterHealthAction : Action
    {
        [BlackboardEnum]
        public enum ActionType
        {
            Heal,
            Damage
        }

        [Tooltip("The action to perform: Heal or Damage.")]
        [SerializeReference] public BlackboardVariable<ActionType> Action;

        [Tooltip("The target character to heal or damage.")]
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [Tooltip("The amount of HP to heal or damage.")]
        [SerializeReference] public BlackboardVariable<int> Amount;

        protected override Status OnStart()
        {
            if (Target.Value == null)
            {
                LogFailure("No target assigned to heal.");
                return Status.Failure;
            }

            var healthComp = Target.Value.GetComponentInChildren<CharacterHealth>();
            if (healthComp == null)
            {
                LogFailure($"Target {Target.Value.name} doesn't have a {typeof(CharacterHealth).Name} component.");
                return Status.Failure;
            }

            if (Action.Value == ActionType.Damage)
            {
                healthComp.ApplyDamage(new DamageInfo()
                {
                    Damage = Amount.Value,
                    DamageMultiplier = 1,
                    Origin = GameObject,
                    IgnoreInvulnerability = false
                });
            }
            else
            {
                healthComp.HealAmount(Amount.Value);
            }

            return Status.Success;
        }
    }
}