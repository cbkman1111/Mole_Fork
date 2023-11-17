using UnityEngine;
using System.Collections;
using Common.Utils.Pool;

public class Done_DestroyByBoundary : MonoBehaviour
{
	void OnTriggerExit (Collider other) 
	{
        PoolManager.Instance.ReturnObject(other.transform);
		//Destroy(other.gameObject);
	}
}