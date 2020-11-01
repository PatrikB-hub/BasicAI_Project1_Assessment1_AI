using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAI : MonoBehaviour
{

    public float minDistance;
    public int index = 0;

    public float minChaseDistance = 1f;
    /*
    void Update()
    {
        //float distanceBetweenAAndB = Vector2.Distance(A, B);
        
        //if we are far from player, Patrol()
        if (Vector2.Distance(player.transform.position, transform.position) > minChaseDistance)
        {
            Patrol();
        }
        else
        {
            MoveAi(player.transform.position);
        }
        //if (gameObject.transform.position >)
        //if the ai is not near the player
        //else
        ////chase the player
        //end if
    }*/

    void Patrol(GameObject[] waypoint, float _patrolSpeed)
    {
        float distance = Vector2.Distance(transform.position, waypoint[index].transform.position);
        
        if(distance < minDistance)
        {
            //index = Random.Range(0, waypoint.Length);
            index++;
        }

        if (index >= waypoint.Length)
        {
            index = 0;
        }

       MoveAi(waypoint[index].transform.position, _patrolSpeed);
    }

    public void ChasePlayer(Transform _chaseTarget, float _chaseSpeed)
    {
        MoveAi(_chaseTarget.transform.position, _chaseSpeed);
    }

    public void FleePlayer(Transform _fleeTarget, float _fleeSpeed)
    {
        MoveAi(-(_fleeTarget.transform.position), _fleeSpeed);
    }

    void MoveAi(Vector2 targetPosition, float _moveSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position,targetPosition , _moveSpeed * Time.deltaTime );
    }

    public void TakeDamage(float _health, float _damage)
    {
        _health -= _damage;
    }

}
