using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePointAI : MonoBehaviour
{
    #region class variables
    public float speed = 5f;
    public GameObject[] Waypoint;
    public float minDistance = 0.5f;
    public float chasePlayerDistance = 5f;
    public int index = 0;
    public GameObject player;
    public PlayerController playerController; // == null
    #endregion
    public enum State//I added states for our ai
    {
        patrol,
        chase,
    }
    public State state;//I added states for our ai
    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        NextState();//I added this line in
    }
    //I DELETED UPDATE FROM WAYPOINTAI
    //CREATE THESE TWO METHODS
    //patrolState()
    //chaseState()
    void Patrol()
    {
        float distance = Vector2.Distance(transform.position, Waypoint[index].transform.position);
        if (distance < minDistance)
        {
            index++;
        }
        if (index >= Waypoint.Length)
        {
            index = 0; ;
        }
        //when we reach waypoint
        //go to next waypoint
        MoveAI(Waypoint[index].transform.position);
    }//From WaypointAI
    void MoveAI(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }//from WaypointAI
    private void NextState()
    {
        //work out the name of the method we want to run
        string methodName = state.ToString() + "State"; //if our current state is "walk" then this returns "walkState"
        //give us a variable so we an run a method using its name
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        //Run our method
        StartCoroutine((IEnumerator)info.Invoke(this, null));
        //Using StartCoroutine() means we can leave and come back to the method that is running
        //All Coroutines must return IEnumerator
    }//from StateMachine
}
