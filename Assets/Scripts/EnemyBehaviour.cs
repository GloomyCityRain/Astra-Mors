using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Movement")]
    private NavMeshAgent agent;
    public Transform player;

    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Vision Cone")]
    private VisionCone visionCone;


    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        visionCone = GetComponentInChildren<VisionCone>();

        StartCoroutine(PatrolRoute());

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveDir = agent.velocity;

        if (moveDir.sqrMagnitude > 0.01f) // only update if moving
        {
            visionCone.facingDir = moveDir.normalized;
        }
        if (visionCone.playerSpotted)
        {
            StartCoroutine(InvestigatePlayer());
            visionCone.playerSpotted = false;
        }
    }

    private IEnumerator PatrolRoute()
    {
        while (!visionCone.playerInside)
        {
            // Set destination to current waypoint
            agent.SetDestination(waypoints[currentWaypoint].position);


            yield return new WaitUntil(() =>
                !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);


            // Small delay
            yield return new WaitForSeconds(1f);

            // Go to next waypoint
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    private IEnumerator InvestigatePlayer()
    {
        agent.SetDestination(player.transform.position);
        agent.isStopped = true;
        yield return new WaitUntil(() =>
            !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        yield return new WaitForSeconds(1f);

        StartCoroutine(PatrolRoute());
    }


}
