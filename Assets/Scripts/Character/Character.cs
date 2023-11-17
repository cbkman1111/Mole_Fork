using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private void Awake()
    {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = transform.GetComponent<NavMeshAgent>();
        }
    }


}
