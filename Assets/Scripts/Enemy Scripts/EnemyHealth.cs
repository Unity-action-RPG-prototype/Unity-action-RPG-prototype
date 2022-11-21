using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] EnemyStatsReference _enemyStatsRef;
    [SerializeField] EnemyHealthBar _enemyHealthBar;
    [SerializeField] EnemyAI _enemyAI;

    public float maxHealth;
    public float currentHealth;

    public float enemyHealth;
    public float CurrentHealth { get => enemyHealth; }

    public void Awake()
    {
        maxHealth = _enemyStatsRef.maxHealth;
        currentHealth = _enemyStatsRef.maxHealth;
    }

    private void Start()
    {
        enemyHealth = currentHealth;
        _enemyHealthBar.UpdateHealthBar(this);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealth();
    }

    public void Assassinated()
    {
        currentHealth = 0;
        _enemyAI.assassinated = true;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        enemyHealth = currentHealth;
        _enemyHealthBar.UpdateHealthBar(this);
    }
}
