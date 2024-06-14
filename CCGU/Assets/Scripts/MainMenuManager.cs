using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        DisableMirrorHUD();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        if (NetworkManager.singleton != null)
        {
            if (NetworkServer.active || NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
                NetworkManager.singleton.StopClient();
            }
        }
        SceneManager.LoadScene("MainMenu");
    }

    private void DisableMirrorHUD()
    {
        var networkManagerHUD = FindObjectOfType<NetworkManagerHUD>();
        if (networkManagerHUD != null)
        {
            networkManagerHUD.enabled = false;
        }
    }
}
