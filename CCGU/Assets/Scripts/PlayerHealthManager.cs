using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHealthManager : NetworkBehaviour
{
    public Text PlayerHealthText;
    public Text EnemyHealthText;

    [SyncVar(hook = nameof(OnPlayerHealthChanged))]
    private int playerHealth = 10;

    [SyncVar(hook = nameof(OnEnemyHealthChanged))]
    private int enemyHealth = 10;

    private void Start()
    {
        UpdateHealthText();
    }

    [Server]
    public void DealDamageToPlayer(int damage)
    {
        playerHealth -= damage;
        if (playerHealth < 0)
            playerHealth = 0;
    }

    [Server]
    public void DealDamageToEnemy(int damage)
    {
        enemyHealth -= damage;
        if (enemyHealth < 0)
            enemyHealth = 0;
    }

    [Server]
    public void HealPlayer(int amount)
    {
        playerHealth += amount;
    }

    [Server]
    public void HealEnemy(int amount)
    {
        enemyHealth += amount;
    }

    private void OnPlayerHealthChanged(int oldHealth, int newHealth)
    {
        UpdateHealthText();
    }

    private void OnEnemyHealthChanged(int oldHealth, int newHealth)
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (PlayerHealthText != null)
        {
            PlayerHealthText.text = "Player Health: " + playerHealth;
        }

        if (EnemyHealthText != null)
        {
            EnemyHealthText.text = "Enemy Health: " + enemyHealth;
        }
    }
}
