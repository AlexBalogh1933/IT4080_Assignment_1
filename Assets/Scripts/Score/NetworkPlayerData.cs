using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class NetworkPlayerData : NetworkBehaviour
{
    public NetworkVariable<int> score = new NetworkVariable<int>();
    public NetworkVariable<ulong> playerNumber = new NetworkVariable<ulong>();
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>();

    public NetworkedPlayers networkedPlayers;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        score.Value = 0;
        playerNumber.Value = GetComponent<NetworkObject>().OwnerClientId;
        GetNameClientRPC(
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] 
                    { 
                        GetComponent<NetworkObject>().OwnerClientId 
                    }
                }
            });
    }

    [ClientRpc]
    private void GetNameClientRPC(ClientRpcParams clientRpcParams)
    {
        string[] first = new string[] { "Steve", "Mark", "Timmy", "Carl", " Alex", "Jimmy", "Carter", "Conner", "Justin" };
        GetNameServerRPC(first[UnityEngine.Random.Range(0, first.Length - 1)]);
    }

    [ServerRpc]
    private void GetNameServerRPC(string pName)
    {
        this.playerName.Value = pName;
    }

}
