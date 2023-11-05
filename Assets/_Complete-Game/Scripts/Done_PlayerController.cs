using UnityEngine;
using System.Collections;
using Scenes;
using Common.Utils.Pool;
using Common.Global;

[System.Serializable]
public class Done_Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour
{
	public float speed;
	public float tilt;
	public Done_Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	 
	private float nextFire;

	public Vector3 Direction { get; set; } = Vector3.zero;
	public SceneInGame scene;

	public void Set(Done_Boundary bound)
	{
		boundary = bound;

    }

    private void OnDestroy()
    {
		scene?.GameOver();
    }

    void Update ()
    {
        if (shotSpawn != null)
        {
       
			if (/*Input.GetButton("Fire1") &&*/ Time.time > nextFire) 
			{
				nextFire = Time.time + fireRate;
				var bolt = PoolManager.Instance.GetObject("Done_Bolt");
				if (bolt != null)
				{
                    bolt.position = shotSpawn.position;
                    bolt.rotation = shotSpawn.rotation;

                    //Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                    SoundManager.Instance.PlayEffect("weapon_player");
                    //GetComponent<AudioSource>().Play ();
                }

            }
        }
    }

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		Vector3 movement = Direction;
        GetComponent<Rigidbody>().velocity = movement * speed;
		
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}
