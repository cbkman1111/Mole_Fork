using UnityEngine;
using UnityEngine.Audio;

namespace Unity.Behavior.Demo
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimation : MonoBehaviour
    {
        [Header("Animate")]
        [SerializeField] private float m_RotationSpeed = 10f;

        [SerializeField] private CharacterHealth m_HealthComponent;
        [SerializeField] private AudioResource m_FootstepCue;

        private Vector3 m_LastPosition;
        private Animator m_Animator;

        public void OnFootstep()
        {
            if (m_FootstepCue == null)
            {
                return;
            }

            AudioSourceFactory.PlayAudio(m_FootstepCue, transform.position, false);
        }

        public void PlayHitAnimation()
        {
            m_Animator.SetTrigger("Hit");
        }

        private void Awake()
        {
            if (m_HealthComponent == null)
            {
                m_HealthComponent = GetComponentInParent<CharacterHealth>();
            }

            m_Animator = GetComponent<Animator>();
            m_LastPosition = transform.position;
        }

        private void LateUpdate()
        {
            Vector3 velocity = (transform.position - m_LastPosition) / Time.deltaTime;
            m_LastPosition = transform.position;

            velocity.y = 0;
            float speed = velocity.magnitude;
            if (speed > 0.5f)
            {
                m_Animator.SetFloat("Speed", speed);

                var dirRot = Quaternion.LookRotation(velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, dirRot, m_RotationSpeed * Time.deltaTime);
            }
            else
            {
                m_Animator.SetFloat("Speed", 0);
            }
        }

        private void OnEnable()
        {
            if (m_HealthComponent != null)
            {
                m_HealthComponent.OnHealthChange += OnHealthChange;
            }
        }

        private void OnDisable()
        {
            if (m_HealthComponent != null)
            {
                m_HealthComponent.OnHealthChange -= OnHealthChange;
            }
        }

        private void OnHealthChange(int currentValue, int newValue)
        {
            if (newValue < currentValue)
            {
                PlayHitAnimation();
            }
        }
    }
}