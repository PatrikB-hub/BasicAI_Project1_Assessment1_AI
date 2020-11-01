using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePointAI : MonoBehaviour
{
    #region class variables

    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 40f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float minDamageDistance = 0.5f;
    [SerializeField] private float minChaseDistance = 5f;
    [SerializeField] private int index = 0;
    [SerializeField] private float waitTime = 1f;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] waypoint;

    public enum State { patrol, chase, flee, wait }
    public State state;

    #endregion

    private void Start()
    {
        NextState();
    }

    private void Update()
    {
        CheckTakeDamage(health, damage);
    }

    #region AI States

    private IEnumerator patrolState()
    {
        Debug.Log("Patrol : Enter");
        while (state == State.patrol)
        {
            if (!CanSeePlayer())
            {
                Patrol(speed);
                yield return 0;
            }
            else
            {
                state = State.chase;
            }
        }
        Debug.Log("Patrol : Exit");
        NextState();
    }

    private IEnumerator chaseState()
    {
        Debug.Log("Chase : Enter");
        while (state == State.chase)
        {
            ChasePlayer(speed / 2);
            yield return 0;
            if (!CanSeePlayer())
            {
                state = State.wait;
            }
        }
        Debug.Log("Chase : Exit");
        NextState();
    }

    private IEnumerator fleeState()
    {
        Debug.Log("Flee : Enter");
        while (state == State.flee)
        {
            FleePlayer(speed * 2);
            yield return 0;
        }
        Debug.Log("Flee : Exit");
        NextState();
    }

    private IEnumerator waitState()
    {
        Debug.Log("Wait : Enter");
        Vector3 positionAtStartWaitTime = transform.position;
        float waitStartTime = Time.time;
        while (state == State.wait)
        {
            Stop(positionAtStartWaitTime);

            if (Time.time > waitStartTime + waitTime)
            {
                if (!CanSeePlayer())
                {
                    state = State.patrol;
                }
            }
            else if (CanSeePlayer())
            {
                state = State.chase;
            }
            yield return 0;
        }
        Debug.Log("Wait : Exit");
        NextState();
    }

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

    #endregion

    #region Moving Methods

    private bool CanSeePlayer()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > minChaseDistance)
        {
            return false;
        }
        return true;
    }

    void Patrol(float _patrolSpeed)
    {
        float distance = Vector2.Distance(transform.position, waypoint[index].transform.position);

        if (distance < minDistance)
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

    public void ChasePlayer(float _chaseSpeed)
    {
        MoveAi(player.transform.position, _chaseSpeed);
    }

    public void FleePlayer(float _fleeSpeed)
    {
        MoveAi(-(player.transform.position), _fleeSpeed);
    }

    void MoveAi(Vector2 targetPosition, float _moveSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
    }

    private void Stop(Vector3 _positionAtStartWaitTime)
    {
        if (transform.position != _positionAtStartWaitTime)
        {
            transform.position = _positionAtStartWaitTime;
        }
    }

    public void CheckTakeDamage(float _health, float _damage)
    {
        if (Vector2.Distance(player.transform.position, transform.position) < minDamageDistance)
        {
            _health -= _damage * Time.deltaTime;
            health = _health;
        }
    }

    #endregion
}
