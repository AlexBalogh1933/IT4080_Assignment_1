using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    // Makes the text viewable with my background
    private static GUIStyle labelStyle;

    private static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }
    
    private static void RunningControls()
    {
        string transportTypeName = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        string serverPort = "?";
        if (transport != null)
        {
            serverPort = $"{transport.ConnectionData.Address}:{transport.ConnectionData.Port}";
        }
        string mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        // Makes the text viewable with my background
        // {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.black;
            labelStyle.hover.textColor = Color.black;
        }

        if (GUILayout.Button($"Shutdown {mode}")) NetworkManager.Singleton.Shutdown();
        GUILayout.Label($"Transport: {transportTypeName} [{serverPort}]", labelStyle);
        GUILayout.Label("Mode: " + mode, labelStyle);
        //}
    }

    public static void GUILayoutNetworkControls()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            RunningControls();
        }
        GUILayout.EndArea();
    }
}
