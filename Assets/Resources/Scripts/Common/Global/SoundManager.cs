using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoSingleton<SoundManager>
{ 
    [SerializeField]
    Hashtable soundTable = new Hashtable();
    private Pool<AudioSource> musics = null;
    private Pool<AudioSource> effect = null;
     
    public override bool Init()
    {
        gameObject.name = string.Format("singleton - {0}", TAG);
        var prefab = ResourcesManager.Instance.Load("Prefabs/Audio Source");
        musics = Pool<AudioSource>.Create(prefab, transform, 1);
        effect = Pool<AudioSource>.Create(prefab, transform, 4);

        LoadSoundClips();
        return true;
    }

    public void LoadSoundClips(){
        AudioClip[] clips = Resources.LoadAll<AudioClip>( "Sounds/" );
        foreach( var clip in clips ){
            soundTable.Add( clip.name, clip );
        }
    }

    private IEnumerator ReturnEffect(AudioSource audio){
		yield return new WaitForSeconds(audio.clip.length);
        audio.gameObject.SetActive(false);
        effect.ReturnObject(audio);
	}

    public void StopAllSound(){
   
    }

    public void PlayEffect( string name, float volume )
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
            audio.Play();

            StartCoroutine(ReturnEffect(audio));
        }
    }
    

    public void PlayMusic(string name, float volume)
    {
        if(soundTable.ContainsKey(name) == false )
            return;

        AudioSource audio = musics.GetObject();
        audio.gameObject.SetActive(true);
        audio.playOnAwake = false;
        audio.loop = true;

        AudioClip clip = (AudioClip)soundTable[name];
        audio.clip = clip;
        audio.Play();
    }
}
