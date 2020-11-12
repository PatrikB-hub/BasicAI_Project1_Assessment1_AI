using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 200f;
    [SerializeField] public float health = 100f;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private float regenHealthValue = 15f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float minDamageDistance = 0.5f;
    [SerializeField] private float minChaseDistance = 5f;

    [SerializeField] private GameObject enemy;

    private Rigidbody2D _playerRigidbody;

    private float _moveHorizontal; // -1 to 1
    private float _moveVertical;   // -1 to 1

    void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckHealthRegen(health, regenHealthValue);
        CheckTakeDamage(health, damage);
    }

    void FixedUpdate()
    {
        _moveHorizontal = Input.GetAxis("Horizontal");
        _moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(_moveHorizontal, _moveVertical);

        _playerRigidbody.AddForce(movement * speed);
    }

    /// <summary>
    /// can I see the player
    /// </summary>
    /// <returns></returns>
    private bool CanSeePlayer()
    {
        if (Vector2.Distance(enemy.transform.position, transform.position) > minChaseDistance)
        {
            return false;
        }
        return true;
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

    /// <summary>
    /// take damage
    /// </summary>
    /// <param name="_health">current health</param>
    /// <param name="_damage">amount of damage to take</param>
    public void CheckTakeDamage(float _health, float _damage)
    {
        if (health > 0)
        {
            if (Vector2.Distance(enemy.transform.position, transform.position) < minDamageDistance)
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


}
