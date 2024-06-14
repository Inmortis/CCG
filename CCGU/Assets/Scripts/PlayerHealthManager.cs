using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHealthManager : NetworkBehaviour
{
    // �������� ������ � ����������
    [SyncVar] public int playerHealth = 20;
    [SyncVar] public int enemyHealth = 20;

    // ������ ��� ����������� ��������
    public Text playerHealthText;
    public Text enemyHealthText;

    // ������������� �������
    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateHealthUI();
    }

    // ������������� �������
    public override void OnStartServer()
    {
        base.OnStartServer();
        UpdateHealthUI();
    }

    // ����� ��� ���������� UI ��������
    [ClientRpc]
    public void RpcUpdateHealthUI()
    {
        playerHealthText.text = "Player Health: " + playerHealth;
        enemyHealthText.text = "Enemy Health: " + enemyHealth;
    }

    // ����� ��� ���������� UI �������� (���������)
    private void UpdateHealthUI()
    {
        playerHealthText.text = "Player Health: " + playerHealth;
        enemyHealthText.text = "Enemy Health: " + enemyHealth;
    }

    // ����� ��� ������� ������
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

    // ����� ��� ������� ����������
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

    // ����� ��� ��������� ����� ������
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

    // ����� ��� ��������� ����� ����������
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
