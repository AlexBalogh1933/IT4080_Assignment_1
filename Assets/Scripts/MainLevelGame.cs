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

    private int colorIndex = 0;
    private Color[] playerColors = new Color[]
    {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
    };

    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(4, 0, 0),
        new Vector3(-4, 0, 0),
        new Vector3(0, 0, 4),
        new Vector3(0, 0, -4)
    };

    private int WrapInt(int curValue, int increment, int max)
    {
        int toReturn = curValue + increment;
        if (toReturn > max)
        {
            toReturn = 0;
        }
        return toReturn;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainlevelCamera.enabled = !IsClient;
        mainlevelCamera.GetComponent<AudioListener>().enabled = !IsClient;
        if (IsServer)
        {
            SpawnPlayers();
        }
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

    private void SpawnPlayers()
    {
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            //Debug.Log("Into Spawning");
            //Player playerSpawn = Instantiate(playerPrefab, NextPosition(), Quaternion.identity);
            //playerSpawn.playerColorNetVar.Value = NextColor();
            //playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

            //Player playerSpawn;
            //if (IsHost && clientId == NetworkManager.LocalClientId)
            //{
            //    Debug.Log("Host Spawn");
            //    playerSpawn = Instantiate(PlayerHost, NextPosition(), Quaternion.identity);
            //}
            //else
            //{
            //    Debug.Log("Client Spawn");
            //    playerSpawn = Instantiate(PlayerDefault, NextPosition(), Quaternion.identity);
            //}

            Player prefab = PlayerDefault;
            if (clientId == NetworkManager.LocalClientId)
            {
                prefab = PlayerHost;
            }
            Player playerSpawn = Instantiate(
                prefab,
                NextPosition(),
                Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            playerSpawn.PlayerColor.Value = NextColor();
        }
    }
}
