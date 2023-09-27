using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHandler : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;
    }

    private void PrintMe()
    {
        if (IsServer)
        {
            NetworkHelper.Log($"I AM a Server! {NetworkManager.ServerClientId}");
        }
        if (IsHost) 
        {
            NetworkHelper.Log($"I AM a Host! {NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if (IsClient)
        {
            NetworkHelper.Log($"I AM a Client! {NetworkManager.LocalClientId}");
        }
        if (!IsServer && !IsClient)
        {
            NetworkHelper.Log("I AM Nothing yet");
        }
    }

    // ---------------------
    // Client Actions
    // ---------------------
    private void OnClientStarted()
    {
        NetworkHelper.Log("!! Client Started !!");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnClientStopped += ClientOnClientStopped;
        PrintMe();
    }

    private void ClientOnClientStopped(bool indicator)
    {
        NetworkHelper.Log("!! Client Stopped !!");
        NetworkManager.OnClientConnectedCallback -= ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ClientOnClientDisconnected;
        NetworkManager.OnClientStopped -= ClientOnClientStopped;
        PrintMe();
    }

    private void ClientOnClientConnected(ulong clientId)
    {
        if (IsClient && clientId == NetworkManager.LocalClientId)
        {
            NetworkHelper.Log($"I have connected {clientId}");
        }
        //PrintMe();
        //// Print I {clientId} have connected to the server
        //// handle the case when we are the client running on the host.
        //// Some other client connected.
        //NetworkHelper.Log($"I client {clientId} has connected to the server");
        //NetworkHelper.Log($"I have connected {clientId}");
    }
    private void ClientOnClientDisconnected(ulong clientId)
    {
        if (IsClient && clientId == NetworkManager.ServerClientId)
        {
            NetworkHelper.Log($"I have disconnected {clientId}");
        }
        //// print I {clientId} have disconnected from the server.
        //NetworkHelper.Log($"I client {NetworkManager.LocalClientId} has disconnected from the server");
        //// Only works if you stop the playing of the game. Does not work when you shutdown Client.
        //NetworkHelper.Log($"I have disconnected {clientId}");
    }



    // ---------------------
    // Server Actions
    // ---------------------
    private void OnServerStarted()
    {
        NetworkHelper.Log("!! Server Started !!");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }

    private void ServerOnServerStopped(bool indicator)
    {
        NetworkHelper.Log("!! Server Stopped !!");
        NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
        NetworkManager.OnServerStopped -= ServerOnServerStopped;
        PrintMe();
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        NetworkHelper.Log($"Client {clientId} connected to the server");
    }
    private void ServerOnClientDisconnected(ulong clientId)
    {
        NetworkHelper.Log($"Client {clientId} disconnected from the server");
    }
}
