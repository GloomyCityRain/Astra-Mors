using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Movement")]
    private NavMeshAgent agent;
    public Transform[] wavePoints;
    public float waitTime = 2f;
    public Transform target; 

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }
    

}
