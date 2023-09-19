using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainLevelGame : NetworkBehaviour
{

    public Player PlayerHost;
    public Player PlayerDefault;
    //public Player playerPrefab;
    public Camera mainlevelCamera;

    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(4, 0, 0),
        new Vector3(-4, 0, 0),
        new Vector3(0, 0, 4),
        new Vector3(0, 0, -4)
    };

    private int colorIndex = 0;
    private Color[] playerColors = new Color[]
    {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
    };

    // Start is called before the first frame update
    void Start()
    {
        mainlevelCamera.enabled = !IsClient;
        mainlevelCamera.GetComponent<AudioListener>().enabled = !IsClient;
        if (IsServer)
        {
            SpawnPlayers();
        }
        //else
        //{
        //    SpawnPlayers(PlayerDefault);
        //    Debug.Log("Clients Spawn");
        //}
    }

    private Vector3 NextPosition()
    {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1)
        {
            positionIndex = 0;
        }
        return pos;
    }

    private Color NextColor()
    {
        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if (colorIndex > playerColors.Length - 1)
        {
            colorIndex = 0;
        }
        return newColor;
    }

    private void SpawnPlayers()
    {
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            Player playerSpawn;
            //Debug.Log("Into Spawning");
            //Player playerSpawn = Instantiate(playerPrefab, NextPosition(), Quaternion.identity);
            //playerSpawn.playerColorNetVar.Value = NextColor();
            //playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            if (IsHost && clientId == NetworkManager.LocalClientId)
            {
                Debug.Log("Host Spawn");
                playerSpawn = Instantiate(PlayerHost, NextPosition(), Quaternion.identity);
            }
            else
            {
                Debug.Log("Client Spawn");
                playerSpawn = Instantiate(PlayerDefault, NextPosition(), Quaternion.identity);
            }
            playerSpawn.playerColorNetVar.Value = NextColor();
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }
    }
}
