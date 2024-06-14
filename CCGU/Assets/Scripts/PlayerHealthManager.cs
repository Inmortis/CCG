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
    public void HealPlayer(int amount, NetworkConnection targetConnection)
    {
        playerHealth += amount;
        TargetUpdateHealth(targetConnection, true, playerHealth);
    }

    [Server]
    public void HealEnemy(int amount, NetworkConnection targetConnection)
    {
        enemyHealth += amount;
        TargetUpdateHealth(targetConnection, false, enemyHealth);
    }

    private void OnPlayerHealthChanged(int oldHealth, int newHealth)
    {
        if (isLocalPlayer)
        {
            UpdateHealthText();
        }
    }

    private void OnEnemyHealthChanged(int oldHealth, int newHealth)
    {
        if (!isLocalPlayer)
        {
            UpdateHealthText();
        }
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

    [TargetRpc]
    void TargetUpdateHealth(NetworkConnection target, bool isPlayer, int newHealth)
    {
        if (isPlayer)
        {
            playerHealth = newHealth;
        }
        else
        {
            enemyHealth = newHealth;
        }
        UpdateHealthText();
    }
}
