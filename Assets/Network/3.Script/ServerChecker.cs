using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public enum Type
{
    Server = 0,
    Client
}

public class ServerChecker : MonoBehaviour
{
    public Type type;
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        if (type.Equals(Type.Server))
        {
            Start_Server();
        }
        else
        {
            Start_Client();
        }

    }

    public void Start_Server()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("WebGL Server ¾ÈµÊ..");
        }
        else
        {
            networkManager.StartServer();
            Debug.Log($"{networkManager.networkAddress} start Server");
            NetworkServer.OnConnectedEvent +=
                (NetworkConnectionToClient) =>
                {
                    Debug.Log($"new client connect : {NetworkConnectionToClient.address}");
                };
            NetworkServer.OnDisconnectedEvent +=
                (NetworkConnectionToClient) =>
                {
                    Debug.Log($"client disconnect : {NetworkConnectionToClient.address}");
                };
        }
    }

    public void Start_Client()
    {
        networkManager.StartClient();
        Debug.Log($"{networkManager.networkAddress} startclient");
    }

    private void OnApplicationQuit()
    {
        if(NetworkClient.isConnected)
        {
            networkManager.StopClient();
        }

        if(NetworkServer.active)
        {
            networkManager.StopServer();
        }
    }
}
