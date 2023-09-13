using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainLevelGame : NetworkBehaviour
{
    public Player playerPrefab;
    public Camera mainlevelCamera;

    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(4, 2, 0),
        new Vector3(-4, 2, 0),
        new Vector3(0, 2, 4),
        new Vector3(0, 2, -4)
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
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            Player playerSpawn = Instantiate(playerPrefab, NextPosition(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }
    }
}
