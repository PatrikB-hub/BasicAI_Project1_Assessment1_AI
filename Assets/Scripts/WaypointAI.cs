using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAI : MonoBehaviour
{
    public float speed = 5.0f;

    public GameObject[] waypoint;
    public float minDistance;
    public int index = 0;

    public float minChaseDistance = 1f;

    public GameObject player;

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
    }

    void Patrol()
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
        //waypoint.Length

       MoveAi(waypoint[index].transform.position);
    }

    void MoveAi(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position,targetPosition , speed * Time.deltaTime );
    }
}
