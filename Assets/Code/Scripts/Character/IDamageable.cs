using UnityEngine;

namespace Unity.Behavior.Demo
{
    public interface IDamageable
    {
        void ApplyDamage(DamageInfo amount);

        void HealAmount(float amount);

        int Health { get; }
        bool IsAlive { get; }
    }

    public struct DamageInfo
    {
        public int Damage;
        public int DamageMultiplier;
        public GameObject Origin;
        public bool IgnoreInvulnerability;
    }
}