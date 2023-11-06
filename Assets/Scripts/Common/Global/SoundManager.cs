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
            AudioSource prefabBGM = ResourcesManager.Instance.LoadInBuild<AudioSource>("Audio Source - BGM");
            AudioSource prefabEffect = ResourcesManager.Instance.LoadInBuild<AudioSource>("Audio Source - Effect");

            musics = Pool<AudioSource>.Create(prefabBGM, transform, 1);
            effect = Pool<AudioSource>.Create(prefabEffect, transform, 5);
            return true;
        }

        public void InitList(Transform root, AudioClip[] list)
        {
            soundTable.Clear();
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
            for (int i = 0; i < musics.ActiveList.Count; i++)
            {
                musics.ActiveList[i].transform.gameObject.SetActive(false);
                musics.ReturnObject(musics.ActiveList[i]);
            }

            for (int i = 0; i < effect.ActiveList.Count; i++)
            {                
                effect.ActiveList[i].transform.gameObject.SetActive(false);
                effect.ReturnObject(effect.ActiveList[i]);
            }
        }

        public void PlayEffect(string name, Vector3 position)
        {
            if( soundTable.ContainsKey( name ) == false )
                return;

            int count = 0;
            foreach (var obj in effect.ActiveList)
            {
                if (obj.name == name)
                {
                    count++;
                }

                if (count > 5)
                {
                    return;
                }
            }

            AudioClip clip = (AudioClip)soundTable[name];
            AudioSource audio = effect.GetObject();
            if (audio != null)
            {                 
                audio.gameObject.SetActive(true);
                audio.playOnAwake = false;
                audio.loop = false;
                audio.clip = clip;
                audio.name = name;
                audio.transform.position = position;
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
