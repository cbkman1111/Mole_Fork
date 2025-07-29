using UnityEngine;
using UnityEngine.Audio;

namespace Unity.Behavior.Demo
{
    public class AudioSourceFactory : MonoBehaviourPool<AudioSource>
    {
        [Header("Audio Source Factory")]
        /// <summary>
        /// Refrence to an AudioSource prefab to use as a model for the instantiated AudioSource.
        /// </summary>
        [Tooltip("Refrence to an AudioSource prefab to use as a model for the instantiated AudioSource.")]
        [SerializeField] private AudioSource m_AudioSourceReference;

        private static AudioSourceFactory m_Instance = null;

        public static void PlayAudio(AudioResource resource, Vector3 location, bool loop = false)
        {
            if (m_Instance == null)
            {
                m_Instance = new GameObject("AudioSourceFactory").AddComponent<AudioSourceFactory>();
            }

            AudioSource audioSource = m_Instance.Get();
            audioSource.transform.position = location;
            audioSource.loop = loop;
            audioSource.resource = resource;
            audioSource.Play();

            if (!loop)
            {
                AudioClip clip = resource as AudioClip;
                if (clip != null)
                {
                    m_Instance.Awaitable_ReleaseAudioClip(audioSource, clip.length);
                }
                else
                {
                    m_Instance.Awaitable_ReleaseAudioResource(audioSource);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            m_Instance = this;
        }

        protected override AudioSource OnProductCreation()
        {
            if (m_AudioSourceReference != null)
            {
                return Instantiate(m_AudioSourceReference, Vector3.zero, Quaternion.identity);
            }

            var audioSource = new GameObject("Audio Source").AddComponent<AudioSource>();
            return audioSource;
        }

        protected override void OnProductReleased(AudioSource product)
        {
            product.enabled = false;
            product.gameObject.SetActive(false);
        }

        protected override void OnGetFromPool(AudioSource product)
        {
            product.enabled = true;
            product.gameObject.SetActive(true);
        }

        protected override void OnProductDestruction(AudioSource product)
        {
            if (product.isPlaying)
            {
                product.Stop();
            }

            Destroy(product);
        }

        private async void Awaitable_ReleaseAudioClip(AudioSource source, float delay)
        {
            await Awaitable.WaitForSecondsAsync(delay);
            ReleaseAudio(source);
        }

        private async void Awaitable_ReleaseAudioResource(AudioSource source)
        {
            do
            {
                await Awaitable.WaitForSecondsAsync(1f);
                
                // If the resource has been destroyed already.
                if (source == null)
                {
                    return;
                }
            }
            while (source.isPlaying);

            ReleaseAudio(source);
        }

        private void ReleaseAudio(AudioSource product)
        {
            if (product.isPlaying)
            {
                product.Stop();
            }

            product.enabled = false;
            product.gameObject.SetActive(false);
            Release(product);
        }
    }
}