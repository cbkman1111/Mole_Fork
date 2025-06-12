using System.Collections;
using System.Collections.Generic;
using Common.Global.Singleton;
using Common.Utils.Pool;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Global
{
    public class SoundManager : MonoSingleton<SoundManager>
    { 
        [SerializeField]
        Hashtable soundTable = new Hashtable();
        private Pool<AudioSource> musics = null;
        private Pool<AudioSource> effect = null;

        private List<AudioSource> listMusic = new List<AudioSource>();
        private List<AudioSource> listEffect = new List<AudioSource>();

        private AudioMixer mixer = null;

        private bool loaded = false;

        protected override bool Init()
        {
            //mixer = ResourcesManager.Instance.LoadInBuild<AudioMixer>("Sound/AudioMixer");
            mixer = ResourcesManager.Instance.LoadAddressable<AudioMixer>("AudioMixer.mixer");

            var prefabBGM = ResourcesManager.Instance.LoadAddressable<GameObject>("Audio Source - BGM.prefab");
            var prefabEffect = ResourcesManager.Instance.LoadAddressable<GameObject>("Audio Source - Effect.prefab");

            musics = Pool<AudioSource>.Create(prefabBGM.GetComponent<AudioSource>(), transform, 1);
            effect = Pool<AudioSource>.Create(prefabEffect.GetComponent<AudioSource>(), transform, 10);
            return true;
        }

        public bool Load()
        {
            if (loaded == true)
                return false;

            AudioClip[] clips = ResourcesManager.Instance.LoadAddressableAll<AudioClip>("Sound");
            foreach (var clip in clips)
            {
                soundTable.Add(clip.name, clip);
            }

            loaded = true;
            return true;
        }

        private IEnumerator ReturnEffect(AudioSource audio){
            yield return new WaitForSeconds(audio.clip.length);
            audio.gameObject.SetActive(false);
            effect.ReturnObject(audio);
        }

        public void StopMusics()
        {
            foreach (var audio in listMusic)
            {
                musics.ReturnObject(audio);
            }

            listMusic.Clear();
        }

        public void StopEffect()
        {
            foreach (var audio in listEffect)
            {
                effect.ReturnObject(audio);
            }

            listEffect.Clear();
        }

        public void PlayEffect( string name)
        {
            if( soundTable.ContainsKey( name ) == false )
                return;

            AudioClip clip = (AudioClip)soundTable[name];
            AudioSource audio = effect.GetObject();
            if(audio != null)
            {
                audio.gameObject.SetActive(true);
                audio.playOnAwake = false;
                audio.loop = false;
                audio.clip = clip;
                audio.name = name;
                audio.Play();

                StartCoroutine(ReturnEffect(audio));
            }
        }
    
        public void PlayMusic(string name)
        {
            if(soundTable.ContainsKey(name) == false )
                return;
            
            AudioSource audio = musics.GetObject();
            if(audio != null)
            {
                audio.gameObject.SetActive(true);
                audio.playOnAwake = false;
                audio.loop = true;

                AudioClip clip = (AudioClip)soundTable[name];
                audio.clip = clip;
                audio.name = name;
                audio.Play();

                listMusic.Add(audio);
            }
        }
    }
}
