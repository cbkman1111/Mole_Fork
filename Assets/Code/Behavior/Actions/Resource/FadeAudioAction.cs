using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Audio;

namespace Unity.Behavior.Demo
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "FadeAudio",
        story: "Fade [VolumeParam] of [AudioMixer] to [TargetVolume] over [Duration] seconds",
        category: "Action/Resource",
        description: "Fades AudioMixer VolumeParam using logarithmic scale.",
        id: "3c72e8a61658e7ad0481923351efa1d0")]
    public partial class FadeAudioAction : Action
    {
        [SerializeReference] public BlackboardVariable<string> VolumeParam;
        [SerializeReference] public BlackboardVariable<AudioMixer> AudioMixer;
        [SerializeReference] public BlackboardVariable<float> Duration;
        [SerializeReference] public BlackboardVariable<float> TargetVolume;

        [CreateProperty] private float m_CurrentTime;
        [CreateProperty] private float m_CurrentVol;
        [CreateProperty] private float m_TargetValue;

        protected override Status OnStart()
        {
            if (AudioMixer.Value == null)
            {
                LogFailure("No AudioMixer assigned.");
                return Status.Failure;
            }

            AudioMixer.Value.GetFloat(VolumeParam.Value, out m_CurrentVol);

            // Source: https://johnleonardfrench.com/blog/
            m_CurrentVol = Mathf.Pow(10, m_CurrentVol / 20);
            m_TargetValue = Mathf.Clamp(TargetVolume, 0.0001f, 1);

            m_CurrentTime = 0;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (m_CurrentTime >= Duration)
            {
                return Status.Success;
            }

            m_CurrentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(m_CurrentVol, m_TargetValue, m_CurrentTime / Duration);
            AudioMixer.Value.SetFloat(VolumeParam.Value, Mathf.Log10(newVol) * 20);

            return Status.Running;
        }
    }
}