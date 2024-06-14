using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    // Карты и зоны на игровом поле
    public GameObject Card1Prefab;
    public GameObject Card2Prefab;
    public GameObject Card3Prefab;
    public GameObject Card4Prefab;
    public GameObject Card5Prefab;
    public GameObject Card6Prefab;
    public GameObject Card7Prefab;
    public GameObject Card8Prefab;
    public GameObject Card9Prefab;
    public GameObject Card10Prefab;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject DropZone;
    public GameObject EnemyDropZone;

    private GameManager gameManager;
    private PlayerHealthManager healthManager;

<<<<<<< Updated upstream
=======
    // Список карт в руке игрока и максимальное количество карт
>>>>>>> Stashed changes
    private List<GameObject> playerHand = new List<GameObject>();
    private const int maxCardsInHand = 9;

    // Инициализация клиента
    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        DropZone = GameObject.Find("DropZone");
        EnemyDropZone = GameObject.Find("EnemyDropZone");

        FindGameManager();
        FindHealthManager();
    }

    // Инициализация сервера
    public override void OnStartServer()
    {
        base.OnStartServer();
        FindGameManager();
        FindHealthManager();
    }

    // Поиск GameManager в сцене
    private void FindGameManager()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager is not found in the scene.");
            }
        }
    }

    // Поиск PlayerHealthManager в сцене
    private void FindHealthManager()
    {
        if (healthManager == null)
        {
            healthManager = FindObjectOfType<PlayerHealthManager>();
            if (healthManager == null)
            {
                Debug.LogError("PlayerHealthManager is not found in the scene.");
            }
        }
    }

    // Команда для раздачи карт
    [Command]
    public void CmdDealCards()
    {
        if (playerHand.Count >= maxCardsInHand)
        {
            Debug.Log("Cannot draw more cards. Hand limit reached.");
            return;
        }

        List<GameObject> cards = new List<GameObject>
        {
            Card1Prefab, Card2Prefab, Card3Prefab, Card4Prefab, Card5Prefab,
            Card6Prefab, Card7Prefab, Card8Prefab, Card9Prefab, Card10Prefab
        };

        for (int i = 0; i < 5; i++)
        {
            if (playerHand.Count >= maxCardsInHand)
            {
                Debug.Log("Cannot draw more cards. Hand limit reached.");
                break;
            }

            GameObject cardPrefab = cards[Random.Range(0, cards.Count)];
            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");

            playerHand.Add(card); // Исправлено с add на Add
        }
    }

    // Воспроизведение карты
    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    // Команда для воспроизведения карты
    [Command]
    public void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");

        // Если карта - Card2, увеличиваем здоровье
        if (card.name == "Card2(Clone)")
        {
            if (isServer)
            {
                NetworkConnection targetConnection = connectionToClient;
                healthManager.HealPlayer(2, targetConnection);
            }
        }

        if (isServer)
        {
            UpdateTurnsPlayed();
        }

        playerHand.Remove(card);

        if (card.name == "Card2(Clone)")
        {
            RpcHandleCard2Played(netIdentity, card);
        }
    }

    // Обновление количества ходов
    [Server]
    private void UpdateTurnsPlayed()
    {
        if (gameManager == null)
        {
            FindGameManager();
        }

        if (gameManager == null)
        {
            Debug.LogError("GameManager is still not found or does not have a GameManager component.");
            return;
        }

        gameManager.UpdateTurnsPlayed();
        RpcLogToClients("Turns Played: " + gameManager.TurnsPlayed);
    }

    // Логирование для клиентов
    [ClientRpc]
    private void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    // Отображение карты на клиенте
    [ClientRpc]
    private void RpcShowCard(GameObject card, string type)
    {
        if (type == "Dealt")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
                card.GetComponent<CardFlipper>().Flip();
            }
        }
        else if (type == "Played")
        {
            if (card.name == "Card1(Clone)")
            {
                DeleteCards(DropZone.transform);
                DeleteCards(EnemyDropZone.transform);
            }

            if (card.name == "Card10(Clone)")
            {
                CmdDrawTwoCards();
            }

            if (card.name == "Card7(Clone)")
            {
                CmdRemoveCardFromDropZone("Card5(Clone)");
            }

            var dropZoneDecided = hasAuthority ? DropZone.transform : EnemyDropZone.transform;
            card.transform.SetParent(dropZoneDecided, false);
            if (!hasAuthority)
            {
                card.GetComponent<CardFlipper>().Flip();
            }

            // Логика для карты Card2
            if (card.name == "Card2(Clone)")
            {
                if (hasAuthority)
                {
                    healthManager.HealPlayer(2, NetworkClient.connection);
                }
                else
                {
                    healthManager.HealEnemy(2, NetworkClient.connection);
                }
            }
        }
    }

    // Команда для добора двух карт
    [Command]
<<<<<<< Updated upstream
    void CmdCreateCardClones(GameObject originalCard)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cardClone = Instantiate(originalCard);
            NetworkServer.Spawn(cardClone);
            RpcAddCardToDropZone(cardClone);
        }
    }

    [ClientRpc]
    void RpcAddCardToDropZone(GameObject card)
    {
        card.transform.SetParent(DropZone.transform, false);
        if (!hasAuthority)
        {
            card.GetComponent<CardFlipper>().Flip();
        }
    }

    [Command]
    void CmdDrawTwoCards()
=======
    private void CmdDrawTwoCards()
>>>>>>> Stashed changes
    {
        for (int i = 0; i < 2; i++)
        {
            if (playerHand.Count >= maxCardsInHand)
            {
                break;
            }

            List<GameObject> cards = new List<GameObject>
            {
                Card1Prefab, Card2Prefab, Card3Prefab, Card4Prefab, Card5Prefab,
                Card6Prefab, Card7Prefab, Card8Prefab, Card9Prefab, Card10Prefab
            };

            GameObject cardPrefab = cards[Random.Range(0, cards.Count)];
            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");

            playerHand.Add(card); // Исправлено с add на Add
        }
    }

    // Команда для удаления карты из DropZone
    [Command]
    private void CmdRemoveCardFromDropZone(string cardName)
    {
        foreach (Transform child in DropZone.transform)
        {
            if (child.gameObject.name == cardName)
            {
                NetworkServer.Destroy(child.gameObject);
                break;
            }
        }
    }

    // Команда для таргетирования своей карты
    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }

    // Команда для таргетирования карты противника
    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity opponentIdentity = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(opponentIdentity.connectionToClient);
    }

    // TargetRpc для таргетирования своей карты
    [TargetRpc]
    private void TargetSelfCard()
    {
        Debug.Log("Targeted by self!");
    }

    // TargetRpc для таргетирования карты противника
    [TargetRpc]
    private void TargetOtherCard(NetworkConnection target)
    {
        Debug.Log("Targeted by other!");
    }

    // Команда для инкремента кликов
    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);
    }

    // ClientRpc для инкремента кликов
    [ClientRpc]
    private void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().NumberOfClicks++;
    }

    // Удаление карт из зоны
    private void DeleteCards(Transform zone)
    {
        foreach (Transform child in zone)
            Destroy(child.gameObject);
    }

    [ClientRpc]
    void RpcHandleCard2Played(NetworkIdentity playerIdentity, GameObject card)
    {
        if (playerIdentity.isLocalPlayer)
        {
            healthManager.HealPlayer(2);
        }
        else
        {
            healthManager.HealEnemy(2);
        }
    }
}
