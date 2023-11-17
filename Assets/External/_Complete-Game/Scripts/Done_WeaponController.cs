using UnityEngine;
using System.Collections;
using Common.Utils.Pool;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Common.Global;

public class Done_WeaponController : MonoBehaviour
{
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;

	public bool Init()
	{
        MEC.Timing.RunCoroutine(Fire(delay), gameObject);
        return true;
	}

	private IEnumerator<float> Fire(float time)
	{
        yield return MEC.Timing.WaitForSeconds(time);

        while (isActiveAndEnabled)
		{
            var fire = PoolManager.Instance.GetObject(shot.name);
            fire.position = shotSpawn.position;
            fire.rotation = shotSpawn.rotation;
			//Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			//GetComponent<AudioSource>().Play();
			SoundManager.Instance.PlayEffect("weapon_enemy", fire.position);
            yield return MEC.Timing.WaitForSeconds(time);
        }
	}

	/*
        void Fire ()
	{
		var fire = PoolManager.Instance.GetObject(shot.name);
		fire.position = shotSpawn.position;
        fire.rotation = shotSpawn.rotation;
        //Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		GetComponent<AudioSource>().Play();
	}
	*/
}
