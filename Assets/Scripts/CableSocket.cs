using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CableSocket : MonoBehaviour
{
    [SerializeField] private int cableSocketId;

    private XRSocketInteractor
        _socketInteractor; // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        _socketInteractor = GetComponent<XRSocketInteractor>();

        if (!_socketInteractor)
        {
            throw new System.Exception("No XRSocketInteractor found on this object");
        }

        _socketInteractor.selectEntered.AddListener(OnCoupled);
        _socketInteractor.selectExited.AddListener(OnDecoupled);
    }

    void OnCoupled(SelectEnterEventArgs selectArgs)
    {
        var ixr = selectArgs.interactableObject;
        var extreme = (ixr as Component).gameObject;
        CableExtreme cableExtreme = extreme.GetComponent<CableExtreme>();
        if (!cableExtreme)
        {
            return;
        }


        cableExtreme.ConnectCableToSocket(this);
    }

    void OnDecoupled(SelectExitEventArgs exitArgs)
    {
        var ixr = exitArgs.interactableObject;
        var extreme = (ixr as Component).gameObject;
        CableExtreme cableExtreme = extreme.GetComponent<CableExtreme>();
        if (!cableExtreme)
        {
            return;
        }

        cableExtreme.DisconnectCableFromSocket();
    }

    public int GetSocketId()
    {
        return cableSocketId;
    }
}