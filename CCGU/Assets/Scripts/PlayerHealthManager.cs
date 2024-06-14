using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHealthManager : NetworkBehaviour
{
    // Здоровье игрока и противника
    [SyncVar] public int playerHealth = 20;
    [SyncVar] public int enemyHealth = 20;

    // Тексты для отображения здоровья
    public Text playerHealthText;
    public Text enemyHealthText;

    // Инициализация клиента
    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateHealthUI();
    }

    // Инициализация сервера
    public override void OnStartServer()
    {
        base.OnStartServer();
        UpdateHealthUI();
    }

    // Метод для обновления UI здоровья
    [ClientRpc]
    public void RpcUpdateHealthUI()
    {
        playerHealthText.text = "Player Health: " + playerHealth;
        enemyHealthText.text = "Enemy Health: " + enemyHealth;
    }

    // Метод для обновления UI здоровья (локальный)
    private void UpdateHealthUI()
    {
        playerHealthText.text = "Player Health: " + playerHealth;
        enemyHealthText.text = "Enemy Health: " + enemyHealth;
    }

    // Метод для лечения игрока
    [Server]
    public void HealPlayer(int amount, NetworkConnection conn)
    {
        if (conn.identity.GetComponent<PlayerManager>().hasAuthority)
        {
            playerHealth += amount;
        }
        else
        {
            enemyHealth += amount;
        }
        RpcUpdateHealthUI();
    }

    // Метод для лечения противника
    [Server]
    public void HealEnemy(int amount, NetworkConnection conn)
    {
<<<<<<< Updated upstream
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
=======
        if (conn.identity.GetComponent<PlayerManager>().hasAuthority)
>>>>>>> Stashed changes
        {
            enemyHealth += amount;
        }
        else
        {
            playerHealth += amount;
        }
        RpcUpdateHealthUI();
    }

    // Метод для нанесения урона игроку
    [Server]
    public void DamagePlayer(int amount, NetworkConnection conn)
    {
        if (conn.identity.GetComponent<PlayerManager>().hasAuthority)
        {
            playerHealth -= amount;
        }
        else
        {
            enemyHealth -= amount;
        }
        RpcUpdateHealthUI();
    }

    // Метод для нанесения урона противнику
    [Server]
    public void DamageEnemy(int amount, NetworkConnection conn)
    {
        if (conn.identity.GetComponent<PlayerManager>().hasAuthority)
        {
            enemyHealth -= amount;
        }
        else
        {
            playerHealth -= amount;
        }
        RpcUpdateHealthUI();
    }
}
