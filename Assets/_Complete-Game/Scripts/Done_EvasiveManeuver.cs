using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Done_EvasiveManeuver : MonoBehaviour
{
	public Done_Boundary boundary;
	public float tilt;
	public float dodge;
	public float smoothing;
	public Vector2 startWait;
	public Vector2 maneuverTime;
	public Vector2 maneuverWait;

	private float currentSpeed;
	private float targetManeuver;

	void Start ()
	{
		currentSpeed = GetComponent<Rigidbody>().velocity.z;

        MEC.Timing.RunCoroutine(Evade());
        MEC.Timing.RunCoroutine(UpdatePosition());
    }

    private IEnumerator<float> Evade()
	{
        yield return MEC.Timing.WaitForSeconds(Random.Range (startWait.x, startWait.y));
		while (true)
		{
			targetManeuver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
            yield return MEC.Timing.WaitForSeconds(Random.Range (maneuverTime.x, maneuverTime.y));
			targetManeuver = 0;
			yield return MEC.Timing.WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
		}
	}

    private IEnumerator<float> UpdatePosition()
    {
        yield return MEC.Timing.WaitForSeconds(0.2f);

        while (true)
        {
            var rigidbody = GetComponent<Rigidbody>();
            float newManeuver = Mathf.MoveTowards(rigidbody.velocity.x, targetManeuver, smoothing * Time.deltaTime);
            var x = Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax);
            var z = Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax);
            rigidbody.velocity = new Vector3(newManeuver, 0.0f, currentSpeed);
            rigidbody.position = new Vector3(x, 0.0f, z);

            rigidbody.rotation = Quaternion.Euler(0, 0, rigidbody.velocity.x * -tilt);
            yield return MEC.Timing.WaitForSeconds(0.1f);
        }
 
	}
}
