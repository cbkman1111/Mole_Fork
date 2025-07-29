using TMPro;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [RequireComponent(typeof(TMP_Text))]
    public class UICharacterHealth : MonoBehaviour
    {
        [SerializeField] private CharacterHealth m_CharacterHealth;
        
        private TMP_Text m_Text;
        
        void OnEnable()
        {
            m_Text = GetComponent<TMP_Text>();
            Debug.Assert(m_CharacterHealth != null, "CharacterHealth not provided.", this);

            m_CharacterHealth.OnHealthChange += OnHealthChange;
            m_Text.text = m_CharacterHealth.Health.ToString();
        }

        private void OnDisable()
        {
            m_CharacterHealth.OnHealthChange -= OnHealthChange;
        }

        private void OnHealthChange(int currentValue, int newValue)
        {
            m_Text.text = newValue.ToString();
        }
    }
}