using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
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

    private List<GameObject> playerHand = new List<GameObject>();
    private const int maxCardsInHand = 9;

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

    public override void OnStartServer()
    {
        base.OnStartServer();
        FindGameManager();
        FindHealthManager();
    }

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

            playerHand.Add(card);
        }
    }

    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    [Command]
    public void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");

        if (card.name == "Card2(Clone)")
        {
            NetworkConnection targetConnection = connectionToClient;
            healthManager.HealPlayer(2, targetConnection);
            RpcHealEnemy(2);
        }

        if (isServer)
        {
            UpdateTurnsPlayed();
        }

        playerHand.Remove(card);
    }

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

    [ClientRpc]
    private void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    [ClientRpc]
    private void RpcShowCard(GameObject card, string type)
    {
        Debug.Log("RpcShowCard called with card: " + card.name + ", type: " + type);

        if (type == "Dealt")
        {
            Debug.Log("Dealt type");
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
            Debug.Log("Played type");

            if (card.name == "Card1(Clone)")
            {
                Debug.Log("First card in the list");
                DeleteCards(DropZone.transform);
                DeleteCards(EnemyDropZone.transform);
            }

            if (card.name == "Card10(Clone)")
            {
                Debug.Log("Card10(Clone) played");
                CmdDrawTwoCards();
            }

            if (card.name == "Card7(Clone)")
            {
                Debug.Log("Card7(Clone) played");
                CmdRemoveCardFromDropZone("Card5(Clone)");
            }

            var dropZoneDecided = hasAuthority ? DropZone.transform : EnemyDropZone.transform;
            card.transform.SetParent(dropZoneDecided, false);
            if (!hasAuthority)
            {
                card.GetComponent<CardFlipper>().Flip();
            }
        }
    }

    [ClientRpc]
    private void RpcHealEnemy(int amount)
    {
        if (!hasAuthority)
        {
            healthManager.HealEnemy(amount, NetworkClient.connection);
        }
    }

    [Command]
    private void CmdDrawTwoCards()
    {
        Debug.Log("CmdDrawTwoCards called");

        for (int i = 0; i < 2; i++)
        {
            if (playerHand.Count >= maxCardsInHand)
            {
                Debug.Log("Cannot draw more cards. Hand limit reached.");
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

            playerHand.Add(card);
        }
    }

    [Command]
    private void CmdRemoveCardFromDropZone(string cardName)
    {
        Debug.Log("CmdRemoveCardFromDropZone called for card: " + cardName);

        foreach (Transform child in DropZone.transform)
        {
            if (child.gameObject.name == cardName)
            {
                Debug.Log("Card found and removed: " + cardName);
                NetworkServer.Destroy(child.gameObject);
                break;
            }
        }
    }

    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }

    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity opponentIdentity = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(opponentIdentity.connectionToClient);
    }

    [TargetRpc]
    private void TargetSelfCard()
    {
        Debug.Log("Targeted by self!");
    }

    [TargetRpc]
    private void TargetOtherCard(NetworkConnection target)
    {
        Debug.Log("Targeted by other!");
    }

    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);
    }

    [ClientRpc]
    private void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().NumberOfClicks++;
        Debug.Log("This card has been clicked " + card.GetComponent<IncrementClick>().NumberOfClicks + " times!");
    }

    private void DeleteCards(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
