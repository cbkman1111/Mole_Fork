using System;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [RequireComponent(typeof(Rigidbody))]
    public class TriggerBehavior : MonoBehaviour
    {
        [SerializeField] private string m_Tag = "Pickup";

        public Rigidbody body { get; private set; }

        public event Action<TriggerBehavior> onTrigger;

        public virtual void OnTrigger(Collider other)
        {
        }

        private void Awake()
        {
            gameObject.tag = m_Tag;

            body = GetComponent<Rigidbody>();
            body.useGravity = false;
            body.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == m_Tag)
            {
                return;
            }

            OnTrigger(other);
            onTrigger?.Invoke(this);
        }
    }
}