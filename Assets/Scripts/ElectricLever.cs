using UnityEngine;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ElectricLever : MonoBehaviour
{
    public event Action OnActivaded;
    public event Action OnDeactivaded;

    private XRGrabInteractable _grabInteractable;

    [SerializeField] private int leverId;

    private HingeJoint _joint;


    private bool _isActive = false;

    [SerializeField] private float treshold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _joint = GetComponentInChildren<HingeJoint>();
        _grabInteractable = GetComponentInChildren<XRGrabInteractable>();
    }

    void Start()
    {
        _grabInteractable.selectEntered.AddListener(OnSelected);
        _grabInteractable.selectExited.AddListener(OnDeselected );
    }

    // Update is called once per frame
    void Update()
    {
        float betwenZeroAndOne = (_joint.angle - _joint.limits.min) / (_joint.limits.max - _joint.limits.min);

        if (!_isActive && betwenZeroAndOne - treshold <= 0)
        {
            Activated();
        }

        if (_isActive && betwenZeroAndOne + treshold >= 1)
        {
            Deactivated();
        }
    }

    void Activated()
    {
        _isActive = true;
        OnActivaded?.Invoke();
        Debug.Log("Activated");
    }

    void Deactivated()
    {
        _isActive = false;
        OnDeactivaded?.Invoke();
        Debug.Log("Deactivated");
    }

    public bool IsActive()
    {
        return _isActive;
    }

    public int GetLeverId()
    {
        return leverId;
    }

    void OnSelected(SelectEnterEventArgs args)
    {
        var controller = args.interactorObject.transform.gameObject;
        var controllerCollider = controller.GetComponent<Collider>();
        if (controllerCollider != null)
        {
            controllerCollider.enabled = false;
        }
    }

    void OnDeselected(SelectExitEventArgs args)
    {
        var controller = args.interactorObject.transform.gameObject;
        var controllerCollider = controller.GetComponent<Collider>();
        if (controllerCollider != null)
        {
            controllerCollider.enabled = true;
        }
        
    }
}