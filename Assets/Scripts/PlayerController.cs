using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed;

    private Rigidbody2D _playerRigidbody;

    private float _moveHorizontal; // -1 to 1
    private float _moveVertical;   // -1 to 1

    void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _moveHorizontal = Input.GetAxis("Horizontal");
        _moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(_moveHorizontal, _moveVertical);

        _playerRigidbody.AddForce(movement * speed);
    }
}
