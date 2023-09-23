using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChatServer : NetworkBehaviour
{
    public ChatUi chatUi;
    const ulong SYSTEM_ID = ulong.MaxValue;
    private ulong[] dmClientIds = new ulong[2];
    private List<ulong> connectedClients = new List<ulong>();

    void Start()
    {
        chatUi.printEnteredText = false;
        chatUi.MessageEntered += OnChatUiMessageEntered;

        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
            NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
            if (IsHost)
            {
                DisplayMessageLocally(SYSTEM_ID, $"You are the host AND client {NetworkManager.LocalClientId}");
                DisplayMessageLocally(SYSTEM_ID, $"Welcome to the server client {NetworkManager.LocalClientId}.");
            }
            else
            {
                DisplayMessageLocally(SYSTEM_ID, "You are the server");
            }
        }
        else
        {
            DisplayMessageLocally(SYSTEM_ID, $"You are a client {NetworkManager.LocalClientId}");
            DisplayMessageLocally(SYSTEM_ID, $"Welcome to the server client {NetworkManager.LocalClientId}.");
        }
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        //ServerSendDirectMessage(
        //    $"I ({NetworkManager.LocalClientId}) see you ({clientId}) have connected to the server, well done",
        //    NetworkManager.LocalClientId,
        //    clientId);
        SendChatMessageServerRpc($"Client ({clientId}) has connected to the server");
        connectedClients.Add(clientId);

    }
    private void ServerOnClientDisconnected(ulong clientId)
    {
        SendChatMessageServerRpc($"Client ({clientId}) has disconnected from the server");
        connectedClients.Remove(clientId);
    }

    private void DisplayMessageLocally(ulong from, string message)
    {
        string fromStr = $"Player {from}";
        Color textColor = chatUi.defaultTextColor;

        if (from == NetworkManager.LocalClientId)
        {
            fromStr = "you";
            textColor = Color.magenta;
        }
        else if (from == SYSTEM_ID)
        {
            fromStr = "SYS";
            textColor = Color.green;
        }
        chatUi.addEntry(fromStr, message, textColor);
    }

    private void OnChatUiMessageEntered(string message)
    {
        SendChatMessageServerRpc(message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        // "@123 Hello world this is a longer message"
        if (message.StartsWith("@"))
        {
            string[] parts = message.Split(" ");
            string clientIdStr = parts[0].Replace("@", "");
            ulong toClientId = ulong.Parse(clientIdStr);

            if (connectedClients.Contains(toClientId))
            {
                ServerSendDirectMessage(message, serverRpcParams.Receive.SenderClientId, toClientId);
            }
            else
            {
                ServerSendDirectMessage(
                    $"Client {toClientId} does not exist. Try again",
                    SYSTEM_ID,
                    serverRpcParams.Receive.SenderClientId);
            }
        }
        else
        {
            ReceiveChatMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);
        }
    }

    [ClientRpc]
    public void ReceiveChatMessageClientRpc(string message, ulong from, ClientRpcParams clientRpcParams = default)
    {
        DisplayMessageLocally(from, message);
    }

    private void ServerSendDirectMessage(string message, ulong from, ulong to)
    {
        // Slow so we removed it
        //ulong[] clientIds = new ulong[2]
        //    {
        //        from, to
        //    };

        // New piece that will make it faster - Premature Optimization
        dmClientIds[0] = from;
        dmClientIds[1] = to;

        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = dmClientIds;

        //clientIds[0] = from;
        //ReceiveChatMessageClientRpc($"<whisper> {message}", from, rpcParams);

        //clientIds[0] = to;
        ReceiveChatMessageClientRpc($"<whisper> {message}", from, rpcParams);
    }
}
