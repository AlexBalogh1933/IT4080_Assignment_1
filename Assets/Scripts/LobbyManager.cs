using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public Button startButton;
    public Button quitButton;
    public TMPro.TMP_Text statusLabel;

    // Start is called before the first frame update
    void Start()
    {
        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        statusLabel.text = "Start something, like the server or the host or the client.";

        startButton.onClick.AddListener(OnStartButtonClicked);
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;

    }

    private void OnServerStarted()
    {
        //StartGame();
        //startButton.gameObject.SetActive(true);
        //quitButton.gameObject.SetActive(true);
        //statusLabel.text = "Press Start";
        GoToLobby();
    }

    private void OnClientStarted()
    {
        if (!IsHost)
        {
            statusLabel.text = "Waiting for game to start";
        }
    }

    private void OnStartButtonClicked()
    {
        StartGame();
    }

    public void GoToLobby()
    {
        NetworkManager.SceneManager.LoadScene(
            "Lobby",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void StartGame()
    {
        NetworkManager.SceneManager.LoadScene(
            "MainLevel", 
            //"TestChat",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
