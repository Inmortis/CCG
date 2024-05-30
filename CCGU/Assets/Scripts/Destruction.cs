using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class DropZoneManager : NetworkBehaviour
{
    public GameObject DropZone;
    public GameObject Card1;

    // Создаем UnityEvent для события, которое будет вызываться при перемещении Card1 в DropZone
    public UnityEvent OnCard1EnteredDropZone;

    // Метод для обработки входа карты Card1 в зону DropZone
    public void OnCard1EnterDropZone()
    {
        // Проверяем, что Card1 находится в зоне DropZone
        if (IsCard1InDropZone())
        {
            Debug.Log("Card1 entered the DropZone.");
            // Если условие выполнено, вызываем событие
            OnCard1EnteredDropZone.Invoke();
        }
    }

    // Метод для проверки, находится ли Card1 в зоне DropZone
    private bool IsCard1InDropZone()
    {
        if (Card1 == null || DropZone == null)
        {
            Debug.LogError("Card1 or DropZone objects are not assigned.");
            return false;
        }

        // Проверяем, находится ли центр коллайдера Card1 внутри коллайдера DropZone
        bool isInDropZone = DropZone.GetComponent<Collider>().bounds.Contains(Card1.transform.position);
        if (isInDropZone)
        {
            Debug.Log("Card1 is in the DropZone.");
        }
        else
        {
            Debug.Log("Card1 is not in the DropZone.");
        }
        return isInDropZone;
    }

    // Сетевая команда для удаления всех карт в зоне DropZone
    [Command]
    private void CmdRemoveCardsInDropZone()
    {
        // Получаем все объекты в зоне DropZone
        Collider[] colliders = Physics.OverlapBox(DropZone.transform.position, DropZone.transform.localScale / 2, Quaternion.identity);

        // Проходимся по всем найденным объектам
        foreach (Collider collider in colliders)
        {
            // Если объект имеет компонент NetworkIdentity, уничтожаем его на всех клиентах
            NetworkIdentity objectIdentity = collider.GetComponent<NetworkIdentity>();
            if (objectIdentity != null)
            {
                // Уничтожаем объект только на сервере, чтобы изменения распространились на всех клиентов
                NetworkServer.Destroy(objectIdentity.gameObject);
            }
        }
    }
}
