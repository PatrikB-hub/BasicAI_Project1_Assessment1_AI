using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePointAI : MonoBehaviour
{
    #region class variables

    [SerializeField] public float health = 100f;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private float regenHealthValue = 15f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float minDamageDistance = 1f;
    [SerializeField] private float minChaseDistance = 5f;
    [SerializeField] private int index = 0;
    [SerializeField] private float waitTime = 1f;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] waypoint;

    private SpriteRenderer sPRenderer;

    public enum State { patrol, chase, flee, wait }
    public State state;

    #endregion

    private void Start()
    {
        sPRenderer = GetComponent<SpriteRenderer>();
        NextState();
    }

    private void Update()
    {
        CheckTakeDamage(health, damage);
        CheckHealthRegen(health, regenHealthValue);
    }

    #region AI States

    private IEnumerator patrolState()
    {
        Debug.Log("Patrol : Enter");
        while (state == State.patrol)
        {
            if (sPRenderer.color != new Color(0, 1, 1))
            {
                Color patrolColour = new Color(0, 1, 1);
                sPRenderer.color = patrolColour;
            }

            // cannot see player then patrol
            if (!CanSeePlayer())
            {
                Patrol(speed);
                yield return 0;
            }
            else // can see player then chase or flee
            {
                if (EnoughHealth())
                {
                    state = State.chase;
                }
                else
                {
                    state = State.flee;
                }
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
            if (sPRenderer.color != new Color(1, 0.2f, 0))
            {
                Color chaseColour = new Color(1, 0.2f, 0);
                sPRenderer.color = chaseColour;
            }

            if (EnoughHealth())
            {
                // chase player
                ChasePlayer(speed * 1.5f);
                yield return 0;
            }
            else // i dont have enough health, can i see player, if so then flee
            {
                if (CanSeePlayer())
                {
                    state = State.flee;
                }
            }
            // cannot see player then wait
            if (!CanSeePlayer())
            {
                state = State.wait;
            }
            else
            {
                if (!EnoughHealth())
                {
                    state = State.flee;
                }
            }
            yield return 0;
        }
        Debug.Log("Chase : Exit");
        NextState();
    }

    private IEnumerator fleeState()
    {
        if (sPRenderer.color != new Color(1, 1, 1))
        {
            Color fleeColour = new Color(1, 1, 1);
            sPRenderer.color = fleeColour;
        }

        Debug.Log("Flee : Enter");
        while (state == State.flee)
        {
            //flee
            FleePlayer(speed);
            yield return 0;
            // can i see the player, if not patrol
            if (!CanSeePlayer())
            {
                state = State.patrol;
            }
            else// i can see the player, do i have enough health, if so then chase
            {
                if (EnoughHealth())
                {
                    state = State.chase;
                }
                yield return 0;
            }
        }
        Debug.Log("Flee : Exit");
        NextState();
    }

    private IEnumerator waitState()
    {
        if (sPRenderer.color != new Color(0, 0, 1))
        {
            Color waitColour = new Color(0, 0, 1);
            sPRenderer.color = waitColour;
        }

        Debug.Log("Wait : Enter");
        Vector3 positionAtStartWaitTime = transform.position;
        // set wait start time
        float waitStartTime = Time.time;
        while (state == State.wait)
        {
            // stop moving
            Stop(positionAtStartWaitTime);
            // health greater than 25%, check for chase or patrol
            if (EnoughHealth())
            {
                // wait for seconds
                if (Time.time > waitStartTime + waitTime)
                {
                    // cannot see player then patrol
                    if (!CanSeePlayer())
                    {
                        state = State.patrol;
                    }
                }
                else if (CanSeePlayer()) // can see player then chase
                {
                    state = State.chase;
                }
            }
            else // health less than 25%, flee
            {
                state = State.flee;
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

    /// <summary>
    /// can I see the player
    /// </summary>
    /// <returns></returns>
    private bool CanSeePlayer()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > minChaseDistance)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// do I have enough health
    /// </summary>
    private bool EnoughHealth()
    {
        if (health > maxHealth * 0.25f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Follows a set path of waypoints
    /// </summary>
    /// <param name="_patrolSpeed">movment speed</param>
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

    /// <summary>
    /// move to players position
    /// </summary>
    /// <param name="_chaseSpeed">speed to chase player</param>
    public void ChasePlayer(float _chaseSpeed)
    {
        MoveAi(player.transform.position, _chaseSpeed);
    }

    /// <summary>
    /// move away from player
    /// </summary>
    /// <param name="_fleeSpeed">speed to move at</param>
    public void FleePlayer(float _fleeSpeed)
    {
        // find the heading towards the player
        Vector3 heading = player.transform.position - transform.position;
        // find its magnitude
        float distance = heading.magnitude;
        // normalise the vector
        Vector3 direction = heading.normalized;

        // depending on the distance to the player move in the opposite direction at a faster or slower speed
        if (distance < 1f)
        {
            Vector3 position = transform.position;
            position += -(direction) * _fleeSpeed * 10f * Time.deltaTime;
            position.z = 0;
            transform.position = position;
        }
        else if (distance < 10f)
        {
            Vector3 position = transform.position;
            position += -(direction) * _fleeSpeed * 3.5f * Time.deltaTime;
            position.z = 0;
            transform.position = position;
        }
        else
        {
            Vector3 position = transform.position;
            position += -(direction) * _fleeSpeed * 1.5f * Time.deltaTime;
            position.z = 0;
            transform.position = position;
        }
    }

    /// <summary>
    /// move towards a certain position
    /// </summary>
    /// <param name="targetPosition">where to move to</param>
    /// <param name="_moveSpeed">how fast to move there</param>
    void MoveAi(Vector2 targetPosition, float _moveSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// stay at the same position
    /// </summary>
    /// <param name="_positionAtStartWaitTime">where to stay</param>
    private void Stop(Vector3 _positionAtStartWaitTime)
    {
        if (transform.position != _positionAtStartWaitTime)
        {
            transform.position = _positionAtStartWaitTime;
        }
    }

    /// <summary>
    /// take damage
    /// </summary>
    /// <param name="_health">current health</param>
    /// <param name="_damage">amount of damage to take</param>
    public void CheckTakeDamage(float _health, float _damage)
    {
        if (health > 0)
        {
            if (Vector2.Distance(player.transform.position, transform.position) < minDamageDistance)
            {
                _health -= _damage * Time.deltaTime;
                health = _health;
            }
        }
        else if (health < 0)
        {
            health = 0;
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// regenerate health
    /// </summary>
    /// <param name="_health">current health</param>
    /// <param name="_regenAmount">how much to regenerate by</param>
    public void CheckHealthRegen(float _health, float _regenAmount)
    {
        if (!CanSeePlayer())
        {
            if (_health < maxHealth)
            {
                _health += _regenAmount * Time.deltaTime;
                health = _health;
            }
            else if (_health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    #endregion
}
