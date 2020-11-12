using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientHealth : MonoBehaviour
{
    private StatePointAI statePointAI = null;
    private PlayerController player = null;

    [SerializeField] private bool onEnemy = false;
    [SerializeField] private bool onPlayer = false;

    public Image healthBar;
    [SerializeField] private Gradient gradient = null;

    void Start()
    {
        if (GetComponent<PlayerController>() != null)
        {
            onPlayer = true;
            onEnemy = false;
            player = GetComponent<PlayerController>();
        }
        if (GetComponent<StatePointAI>() != null)
        {
            onEnemy = true;
            onPlayer = false;
            statePointAI = GetComponent<StatePointAI>();
        }

    }

    void Update()
    {
        if (onPlayer)
        {
            SetHealth(player.health, player.maxHealth);
        }
        else if (onEnemy)
        {
            SetHealth(statePointAI.health, statePointAI.maxHealth);
        }
    }

    public void SetHealth(float _health, float _maxHealth)
    {
        healthBar.fillAmount = Mathf.Clamp01(_health / _maxHealth);

        if (onEnemy)
        {
            healthBar.color = gradient.Evaluate(healthBar.fillAmount);
        }
    }




}
