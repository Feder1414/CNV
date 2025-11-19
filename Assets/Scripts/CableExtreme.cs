using System;
using System.Net.Sockets;
using UnityEngine;

public class CableExtreme : MonoBehaviour
{
    [SerializeField] private int ropeId;
    public event Action OnConnectedCableToSocket;

    public event Action OnDisconnectedCableFromSocket;
    private CableSocket _connectedCableSocket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void ConnectCableToSocket(CableSocket cableSocket)
    {
        _connectedCableSocket = cableSocket;
        OnConnectedCableToSocket?.Invoke();
        Debug.Log($"Cable with id {this.ropeId} connected to socket {_connectedCableSocket.GetSocketId()}");
    }

    public void DisconnectCableFromSocket()
    {
        Debug.Log($"Cable with {ropeId} disconnected from socket {_connectedCableSocket.GetSocketId()}");
        _connectedCableSocket = null;
        OnDisconnectedCableFromSocket?.Invoke();
    }

    public int GetIdSocketConected()
    {
        if (!_connectedCableSocket)
        {
            return -1;
        }

        return _connectedCableSocket.GetSocketId();
    }

    public int GetRopeId()
    {
        return this.ropeId;
    }
}