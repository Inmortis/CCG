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

    [Server]
    public override void OnStartServer()
    {
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

            playerHand.Add(card); // Исправлено с add на Add
        }
    }

    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");

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

    [Server]
    void UpdateTurnsPlayed()
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
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
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
        }
    }

    [Command]
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

    [Command]
    void CmdRemoveCardFromDropZone(string cardName)
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
    void TargetSelfCard()
    {
        Debug.Log("Targeted by self!");
    }

    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
        Debug.Log("Targeted by other!");
    }

    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);
    }

    [ClientRpc]
    void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().NumberOfClicks++;
    }

    void DeleteCards(Transform zone)
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
