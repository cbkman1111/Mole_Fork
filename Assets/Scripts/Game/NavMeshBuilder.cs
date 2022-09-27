using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBuilder : MonoBehaviour
{
	//NavMeshSurface surface = null;

	public void GenerateNavmesh()
	{
		NavMeshSurface[] surfaces = gameObject.GetComponentsInChildren<NavMeshSurface>();
		foreach (var s in surfaces)
		{
			s.RemoveData();
			s.BuildNavMesh();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			GenerateNavmesh();
		}
	}
}
