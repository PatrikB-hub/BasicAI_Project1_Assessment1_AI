using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public enum State
    {
        crawl,
        walk,
        run,
    }

    public State state;

    public int myInt = 0;

    private IEnumerator crawlState()
    {
        float startTime = Time.time;
        Debug.Log("crawl: Enter");
        while(state == State.crawl)
        {
            yield return null;//come back to this method next frame
        }
        Debug.Log("We were crawling for" + (Time.time - startTime) + "seconds");
        Debug.Log("crawl: Exit");
        NextState();
    }
    private IEnumerator walkState()
    {
        Debug.Log("walk: Enter");
        //come back after 5 seconds
        //yield return new WaitForSeconds(5f);
        while (state == State.walk)
        {
            Debug.Log("AHHHHHHHHHHHH");
            yield return null;
        }
        Debug.Log("walk: Exit");
        NextState();
    }
    private IEnumerator runState()
    {
        Debug.Log("run: Enter");        
        while (state == State.run)
        {
            yield return null;
        }
        Debug.Log("run: Exit");
        NextState();
    }

    private void Start()
    {
        NextState();
    }

    private void NextState()
    {
        //work out the sname of the method we want to run
        string methodName = state.ToString() + "State"; //if our current state is walk this returns walkState
        //give us a variable so we can run a method using its name
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        //Run our method
        StartCoroutine((IEnumerator)info.Invoke(this, null));
        //Using StartCoroutine() means we can leave and come back to the method that is running
        //All Coroutines must return IEnumerator
    }
}
