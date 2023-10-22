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

    private NetworkedPlayers networkedPlayers;

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

        networkedPlayers = GameObject.Find("NetworkedPlayers").GetComponent<NetworkedPlayers>();
        NetworkHelper.Log($"Player = {networkedPlayers.allNetPlayers.Count}");

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
        foreach (NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            Player prefab = PlayerDefault;
            if (info.clientId == NetworkManager.LocalClientId)
            {
                prefab = PlayerHost;
            }
            Player playerSpawn = Instantiate(
                prefab,
                NextPosition(),
                Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(info.clientId);
            playerSpawn.PlayerColor.Value = info.color;
        }
    }
}
