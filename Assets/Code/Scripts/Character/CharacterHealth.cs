using System.Collections;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    public class CharacterHealth : MonoBehaviour, IDamageable, ICharacterStateChannelModifier
    {
        public int Health
        {
            get => m_Health;
            private set
            {
                if (m_Health == value)
                {
                    return;
                }

                OnHealthChange?.Invoke(m_Health, value);
                m_Health = value;
            }
        }

        public bool IsAlive { get; private set; }
        public bool isVulnerable { get; private set; }

        public delegate void HealthChangeDelegate(int currentValue, int newValue);

        public event HealthChangeDelegate OnHealthChange;

        public CharacterStateEventChannel StateChannel { get; set; }

        [SerializeField] private float m_InvulnerabilityDuration = 0.5f;
        [SerializeField] private int m_InitialHealth = 3;
        [SerializeField] private int m_MaxHealth = 100;
        [Tooltip("The character does not die when reaching 0 health point.")]
        [SerializeField] private bool m_isImmortal = false;

        private float m_RemainingInvulnerabilityTime = 0f;
        private int m_Health;

        public void ApplyDamage(DamageInfo info)
        {
            if (IsAlive == false || isVulnerable == false)
            {
                return;
            }

            Health = Mathf.Max(Health - (info.Damage * info.DamageMultiplier), 0);

            if (m_isImmortal == false && Health == 0)
            {
                IsAlive = false;
                StateChannel?.SendEventMessage(CharacterState.Dead);
            }
            else
            {
                StateChannel?.SendEventMessage(CharacterState.Hit);

                if (info.Damage > 0 && info.IgnoreInvulnerability == false)
                {
                    m_RemainingInvulnerabilityTime = m_InvulnerabilityDuration;

                    if (isVulnerable)
                    {
                        isVulnerable = false;
                        StartCoroutine(InvulnerabilityRoutine());
                    }
                }
            }
        }

        public void HealAmount(float amount)
        {
            if (IsAlive == false || isVulnerable == false)
            {
                return;
            }

            Health = (int)Mathf.Min(Health + amount, m_MaxHealth);
        }

        private void OnEnable()
        {
            Health = m_InitialHealth;
            IsAlive = true;
            isVulnerable = true;
        }

        private IEnumerator InvulnerabilityRoutine()
        {
            while (m_RemainingInvulnerabilityTime > 0)
            {
                yield return new WaitForFixedUpdate();
                m_RemainingInvulnerabilityTime -= Time.fixedDeltaTime;
            }

            StateChannel?.SendEventMessage(CharacterState.Idle);
            isVulnerable = true;
        }

#if UNITY_EDITOR

        [ContextMenu("Debug_Heal")]
        private void Debug_Heal()
        {
            HealAmount(m_MaxHealth);
        }

        [ContextMenu("Debug_Damage")]
        private void Debug_Damage()
        {
            ApplyDamage(new DamageInfo()
            {
                Damage = 1,
                DamageMultiplier = 1,
                IgnoreInvulnerability = false,
                Origin = this.gameObject
            });
        }

#endif
    }
}