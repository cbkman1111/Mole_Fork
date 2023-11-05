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
        private AudioMixer mixer = null;

        //private Transform root = null;

        
        protected override bool Init()
        {
            mixer = ResourcesManager.Instance.LoadInBuild<AudioMixer>("AudioMixer");
            AudioSource prefab = ResourcesManager.Instance.LoadInBuild<AudioSource>("Audio Source");
            
            musics = Pool<AudioSource>.Create(prefab, transform, 2);
            effect = Pool<AudioSource>.Create(prefab, transform, 5);
  
            
            return true;
        }

        public void InitList(Transform root, AudioClip[] list)
        {
            soundTable.Clear();
            //this.root = root;

            foreach (var clip in list)
            {
                soundTable.Add(clip.name, clip);
            }
        }

        /*
        public bool Load()
        {
            AudioClip[] clips = ResourcesManager.Instance.LoadAllInBuild<AudioClip>("Sounds");
            foreach (var clip in clips)
            {
                soundTable.Add(clip.name, clip);
            }

            return true;
        }
        */

        private IEnumerator ReturnEffect(AudioSource audio){
            yield return new WaitForSeconds(audio.clip.length);

            audio.gameObject.SetActive(false);
            effect.ReturnObject(audio);
        }

        public void StopAllSound()
        {
            musics.ReturnAll();
            effect.ReturnAll();
        }

        public void PlayEffect(string name)
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
            }
        }
    }
}
