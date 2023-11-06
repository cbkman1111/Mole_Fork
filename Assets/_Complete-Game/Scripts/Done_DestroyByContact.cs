using UnityEngine;
using System.Collections;
using Scenes;
using Common.Utils.Pool;
using Common.Global;

public class Done_DestroyByContact : MonoBehaviour
{
	public int scoreValue;

	void Start ()
	{

	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" )
			return;

		if (tag == "Enemy" && other.tag == "Enemy_Bullet")
			return;

        if (tag == "Enemy_Bullet" && other.tag == "Enemy")
            return;

        if (tag == "Enemy" && other.tag == "Enemy")
            return;




        if (other.tag == "Player")
		{
            var explosionPlayer = PoolManager.Instance.GetObject("done_explosion_player");
            explosionPlayer.position = transform.position;
            explosionPlayer.rotation = transform.rotation;
            SoundManager.Instance.PlayEffect("explosion_asteroid", other.transform.position);
		}
        else
        {
            var explosion = PoolManager.Instance.GetObject("done_explosion_asteroid");
            explosion.position = transform.position;
            explosion.rotation = transform.rotation;
            SoundManager.Instance.PlayEffect("explosion_asteroid", explosion.transform.position);

            PoolManager.Instance.ReturnObject(transform);
        }

        //PoolManager.Instance.ReturnObject(other.transform);
        
    }
}