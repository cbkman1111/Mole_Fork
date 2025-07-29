using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Generate Camera Impulse",
        story: "Generate Camera Impulse with of force [Force]",
        category: "Action/Cinemachine",
        id: "c261f6a8ee400a8339be27be39485490")]
    public partial class RumbleCameraAction : Action
    {
        private static Stack<CinemachineImpulseSource> s_SharedPool = null;

        [SerializeReference] public BlackboardVariable<float> Force;

        [Tooltip("Prefab or scene reference use as model to configure the instantiated Cinamechine Impulse Source.")]
        [SerializeReference] public BlackboardVariable<CinemachineImpulseSource> ImpulseSourceReference;

        protected override Status OnStart()
        {
            CinemachineImpulseSource cinemachineImpulseSource = GetCinemachineImpulseSource();

            if (ImpulseSourceReference.Value != null)
            {
                cinemachineImpulseSource.ImpulseDefinition = ImpulseSourceReference.Value.ImpulseDefinition;
            }
            else
            {
                cinemachineImpulseSource.ImpulseDefinition.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble;
            }

            cinemachineImpulseSource.GenerateImpulse(Force.Value);

            Awaitable_ReleaseImpulseSource(cinemachineImpulseSource, cinemachineImpulseSource.ImpulseDefinition.ImpulseDuration);

            return Status.Success;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void ResetStaticInstance()
        {
            s_SharedPool = new Stack<CinemachineImpulseSource>();
        }

        private static CinemachineImpulseSource GetCinemachineImpulseSource()
        {
            if (s_SharedPool == null)
            {
                s_SharedPool = new Stack<CinemachineImpulseSource>();
            }

            CinemachineImpulseSource source = null;
            if (s_SharedPool.Count > 0)
            {
                do
                {
                    source = s_SharedPool.Pop();
                } // Skip audio sources we lost reference to.
                while (source == null && s_SharedPool.Count > 0);

                if (source != null)
                {
                    return source;
                }
            }

            return GameObject.Instantiate(new GameObject("CinemachineImpulseSource")).AddComponent<CinemachineImpulseSource>();
        }

        private void ReleaseImpulseSource(CinemachineImpulseSource source)
        {
            source.enabled = false;
            source.gameObject.SetActive(false);

            if (s_SharedPool == null)
            {
                s_SharedPool = new Stack<CinemachineImpulseSource>();
            }
            s_SharedPool.Push(source);
        }

        private async void Awaitable_ReleaseImpulseSource(CinemachineImpulseSource source, float delay)
        {
            await Awaitable.WaitForSecondsAsync(delay);
            ReleaseImpulseSource(source);
        }
    }
}
