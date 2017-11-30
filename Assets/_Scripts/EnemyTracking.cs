using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTracking : MonoBehaviour {

    public Transform target;

	void Start () {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
