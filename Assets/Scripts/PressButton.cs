using System;
using System.Text;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    [SerializeField] private float treshold = 0.1f;
    [SerializeField] private float deadZone = 0.025F;
    [SerializeField] private bool checkValues = false;

    public event Action OnPresssed;

    public event Action OnReleased;

    private ConfigurableJoint _configurableJoint;

    private Vector3 _startPosition;

    private bool _isPressed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _configurableJoint = GetComponent<ConfigurableJoint>();

        if (!_configurableJoint)
        {
            throw new SystemException("There is no configurable joint attached to the game object of this button");
        }
    }

    void Start()
    {
        _startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkValues)
        {
            Debug.Log(GetValue());
        }

        if (!_isPressed && GetValue() + treshold >= 1)
        {
            Pressed();
        }

        if (_isPressed && GetValue() - treshold <= 0)
        {
            Released();
        }
    }

    float GetValue()
    {
        var value = Vector3.Distance(_startPosition, transform.localPosition) / _configurableJoint.linearLimit.limit;

        if (Math.Abs(value) < deadZone)
        {
            return 0;
        }

        return Math.Clamp(value, -1, 1);
    }

    private void Pressed()
    {
        _isPressed = true;
        OnPresssed?.Invoke();
        Debug.Log("Pressed");
    }

    private void Released()
    {
        _isPressed = false;
        OnReleased?.Invoke();
        Debug.Log("Released");
    }

    public bool IsPressed()
    {
        return _isPressed;
    }

    public void SeeHandlers()
    {
        StringBuilder sb = new StringBuilder();
        if (OnReleased == null)
        {
            Debug.Log("There is no handlers for button released");
        }

        else
        {
            sb.Append("Handlers of the event ");
            foreach (var @delegate in OnReleased.GetInvocationList())
            {
                sb.Append(@delegate.Method.Name + ", ");
            }

            sb.Append(" ");
            Debug.Log(sb.ToString());
            sb.Clear();
        }

        if (OnPresssed == null)
        {
            Debug.Log("There is no handlers for button pressed");
        }
        else
        {
            sb.Append("Handlers of the event pressed");
            foreach (var @delegate in OnPresssed.GetInvocationList())
            {
                sb.Append(@delegate.Method.Name + ", ");
            }

            sb.Append(" ");
            Debug.Log(sb.ToString());
            sb.Clear();
        }
    }
}