using UnityEngine;

namespace Unity.Behavior.Demo
{
    /// <summary>
    /// Controller wrapping a Character's BehaviorGraph StateMachine.
    /// </summary>
    public abstract class AgentController : MonoBehaviour
    {
        public static readonly string k_StateChannelVariableName = "Character State Channel";

        public enum EnableInputBehaviour
        {
            OnAwake,
            OnEnable,
            OnStart,
            Manual
        }

        public enum DisableInputBehaviour
        {
            OnDisable,
            Manual
        }

        [Header("Controller")]
        [SerializeField]
        private GameObject m_ControlledCharacter;

        [SerializeField, Tooltip("The behavior graph running the character logic.")]
        private BehaviorGraphAgent m_StateMachine;

        [SerializeField]
        private EnableInputBehaviour m_enableInputBehaviour = EnableInputBehaviour.OnAwake;

        [SerializeField, Tooltip("DisableInput is always called OnDestroy.")]
        private DisableInputBehaviour m_disableInputBehaviour = DisableInputBehaviour.Manual;

        [SerializeField, Tooltip("Is the controller use in standalone without a character?")]
        private bool m_IsStandalone = false;

        public GameObject ControlledCharacter => m_ControlledCharacter;

        public CharacterStateEventChannel StateChannel => m_StateChannel;
        private CharacterStateEventChannel m_StateChannel;

        public bool IsStandalone => m_IsStandalone;

        public abstract bool IsAI { get; }

        public abstract void EnableInput();

        public abstract void DisableInput();

        protected virtual void Awake()
        {
            if (m_enableInputBehaviour == EnableInputBehaviour.OnAwake)
            {
                EnableInput();
            }

            BindCharacterStateChannel();
        }

        protected virtual void Start()
        {
            if (m_enableInputBehaviour == EnableInputBehaviour.OnStart)
            {
                EnableInput();
            }
        }

        protected virtual void UpdateController(float deltaTime)
        {

        }

        protected virtual void OnEnable()
        {
            if (m_enableInputBehaviour == EnableInputBehaviour.OnEnable)
            {
                EnableInput();
            }
        }

        protected virtual void OnDisable()
        {
            if (m_disableInputBehaviour == DisableInputBehaviour.OnDisable)
            {
                DisableInput();
            }
        }

        /// <summary>
        /// Get the local Character's EventChannel BlackboardVariable instance to propagate it to the C# systems.
        /// Note: In case a BlackboardVariable of type EventChannel is not assigned,
        /// it is generated per instance by the Behavior(Graph)Agent.
        /// </summary>
        private void BindCharacterStateChannel()
        {
            Debug.Assert(m_StateMachine != null, "AgentController need a valid StateMachine", this);

            if (!m_StateMachine.BlackboardReference.GetVariableValue(k_StateChannelVariableName, out m_StateChannel))
            {
                Debug.LogError($"{m_StateMachine} is expecting a BlackboardVariable of type '{typeof(CharacterStateEventChannel).Name}' named " +
                    $"{k_StateChannelVariableName}.");
                return;
            }

            m_StateChannel.name = this.transform.parent.name + " State Channel";
            var components = transform.parent.GetComponentsInChildren<ICharacterStateChannelModifier>();
            if (components == null || components.Length == 0)
            {
                return;
            }

            foreach (var component in components)
            {
                component.StateChannel = m_StateChannel;
            }
        }
    }

    public abstract class AgentController<T> : AgentController
        where T : AgentControllerModule
    {
        [Header("Modules")]
        [SerializeField] protected T[] m_Modules;

        [SerializeField] private bool m_AutoCaptureModule = true;

        public bool TryGetModule<ModuleType>(out ModuleType outModule) where ModuleType : AgentControllerModule
        {
            outModule = null;
            for (int i = 0, c = m_Modules.Length; i < c; ++i)
            {
                var module = m_Modules[i];
                if (module.GetType() == typeof(ModuleType))
                {
                    outModule = module as ModuleType;
                    return true;
                }
            }

            return false;
        }

        protected override void Awake()
        {
            if (!ControlledCharacter && !IsStandalone)
            {
                Debug.LogWarning($"No Character set on {this}, searching for one in the parent hierarchy...");
                Debug.Assert(ControlledCharacter, $"No Character found for {this.transform.parent.gameObject.name}/{this}");
            }

            if (m_AutoCaptureModule)
            {
                CaptureCharacterControllerModules();
            }

            foreach (var module in m_Modules)
            {
                module.InitModule(this);
            }

            base.Awake();
        }

        protected virtual void OnDestroy()
        {
            DisableInput();
        }

        protected override void UpdateController(float deltaTime)
        {
            if (!IsStandalone && ControlledCharacter == null)
            {
                return;
            }

            foreach (var module in m_Modules)
            {
                if (!module.ShouldUpdate())
                {
                    continue;
                }

                module.UpdateModule(deltaTime);
            }
        }

        protected override void OnEnable()
        {
            foreach (var module in m_Modules)
            {
                module.enabled = true;
            }

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            foreach (var module in m_Modules)
            {
                module.enabled = false;
            }

            base.OnDisable();
        }

        protected virtual void OnValidate()
        {
            if (m_AutoCaptureModule)
            {
                CaptureCharacterControllerModules();
            }
        }

        protected virtual void CaptureCharacterControllerModules()
        {
            m_Modules = GetComponents<T>();
        }
    }
}