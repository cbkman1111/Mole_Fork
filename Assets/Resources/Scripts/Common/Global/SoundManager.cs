using Singleton;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{ 
    [SerializeField]
    Hashtable soundTable = new Hashtable();
    private Pool<AudioSource> musics = null;
    private Pool<AudioSource> effect = null;
    private AudioMixer mixer = null;

    public override bool Init()
    {
        gameObject.name = string.Format("singleton - {0}", TAG);

        mixer = ResourcesManager.Instance.Load<AudioMixer>("AudioMixer");

        AudioSource prefab = ResourcesManager.Instance.Load<AudioSource>("Prefabs/Audio Source");
        musics = Pool<AudioSource>.Create(prefab, transform, 1);
        effect = Pool<AudioSource>.Create(prefab, transform, 10);

        LoadSoundClips();
        return true;
    }

    public void LoadSoundClips(){
        AudioClip[] clips = ResourcesManager.Instance.LoadAll<AudioClip>("Sounds/");
        foreach( var clip in clips ){
            soundTable.Add(clip.name, clip);
        }
    }

    private IEnumerator ReturnEffect(AudioSource audio){
		yield return new WaitForSeconds(audio.clip.length);
        audio.gameObject.SetActive(false);
        effect.ReturnObject(audio);
	}

    public void StopAllSound()
    {
   
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
        }
    }
}
