using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleSystem : MonoBehaviour, EventSubmarine
{
    [SerializeField] ConsoleSystemStep[] consoleSystemSteps;
    [SerializeField] private ElectricLever lever;

    [SerializeField] private PressButton button;

    [SerializeField] private CableExtreme[] ropes;

    private Dictionary<int, CableExtreme> _idToCable;

    private event Action onStepCompleted;

    private int _currStep;

    private Action _onLeverActivated;
    private Action _onLeverDeactivated;

    private Action _onButtonPressed;
    private Action _onButtonReleased;

    private Dictionary<int, Action> _onCableConnected;
    private Dictionary<int, Action> _onCableDisconnected;

    private int _amountCorrectStatus = 0;

    private bool _isInStepSequence = false;

    private bool _currStepCompleted = false;

    [SerializeField] private PressButton startButton;
    [SerializeField] private LightSuccess[] lightSuccess;

    public event Action eventCompleted;

    [SerializeField] private int eventId;

    [SerializeField] AudioClip startAudioClip;

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip failedAudioClip;

    [SerializeField] AudioClip successAudioClip;

    private float _startTime;

    public int EventId
    {
        get => eventId;
        set => eventId = value;
    }

    public float TotalTime { get; set; }

    public int TotalErrors { get; set; }

    private bool _isCompleted = false;

    public bool canStartEvent { get; set; }

    private bool _firstTry = true;

    private void Awake()
    {
        if (ropes.Length == 0 || ropes == null)
        {
            throw new SystemException("No ropes found on this object");
        }

        if (lightSuccess.Length != consoleSystemSteps.Length)
        {
            throw new SystemException("There are no same amount of succes lights for the amount of steps");
        }

        _idToCable = new Dictionary<int, CableExtreme>();
        _onCableConnected = new Dictionary<int, Action>();
        _onCableDisconnected = new Dictionary<int, Action>();
        foreach (var cableExtreme in ropes)
        {
            _idToCable[cableExtreme.GetRopeId()] = cableExtreme;
            _onCableConnected[cableExtreme.GetRopeId()] = null;
            _onCableDisconnected[cableExtreme.GetRopeId()] = null;
        }

        onStepCompleted += OnStepCompleted;

        startButton.OnPresssed += StartStepSequence;

        if (!audioSource)
        {
            throw new SystemException("No audio source found on this object");
        }

        if (!startAudioClip || !failedAudioClip || !successAudioClip)
        {
            throw new SystemException("All Audio clips or some are not set on this object");
        }
    }

    private void Start()
    {
        EventManager.instance.RegisterEvent(eventId, this);
    }

    void StartStep(ConsoleSystemStep step)
    {
        UnhookEverything();
        if (_isCompleted)
        {
            return;
        }

        _amountCorrectStatus = 0;
        _currStepCompleted = false;
        Debug.Log($"Starting step {_currStep}");
        if (CheckCorrectStatus(step.leverActivated, lever.IsActive()))
        {
            _amountCorrectStatus += 1;
            CheckStepCompleted(step);
        }

        Debug.Log(
            $"Check button already in desired stated: Curr state{button.IsPressed()}, desiredState {step.buttonActivated}");
        if (CheckCorrectStatus(step.buttonActivated, button.IsPressed()))
        {
            _amountCorrectStatus += 1;
            CheckStepCompleted(step);
        }

        _onLeverActivated = () =>
        {
            Debug.Log($"Callback for  lever activated");
            bool isCorrect = CheckCorrectStatus(step.leverActivated, lever.IsActive());
            if (!isCorrect)
            {
                OnFailedStep();
                return;
            }

            _amountCorrectStatus += 1;
            CheckStepCompleted(step);
        };

        _onLeverDeactivated = () =>
        {
            Debug.Log($"Callback for  lever deactivated");
            bool isCorrect = CheckCorrectStatus(step.leverActivated, lever.IsActive());
            if (!isCorrect)
            {
                OnFailedStep();
                return;
            }

            _amountCorrectStatus += 1;
            CheckStepCompleted(step);
        };

        _onButtonPressed = () =>
        {
            bool isCorrect = CheckCorrectStatus(step.buttonActivated, button.IsPressed());
            if (!isCorrect)
            {
                OnFailedStep();
                return;
            }

            _amountCorrectStatus += 1;
            CheckStepCompleted(step);
        };

        _onButtonReleased = () =>
        {
            bool isCorrect = CheckCorrectStatus(step.buttonActivated, button.IsPressed());
            if (!isCorrect)
            {
                OnFailedStep();
                return;
            }

            _amountCorrectStatus += 1;
            CheckStepCompleted(step);
        };

        lever.OnActivaded += _onLeverActivated;
        lever.OnDeactivaded += _onLeverDeactivated;

        button.OnPresssed += _onButtonPressed;
        button.OnReleased += _onButtonReleased;

        HashSet<int> cableIdsWithSteps = new HashSet<int>();
        for (int i = 0; i < step.connectedCables.Length; i++)
        {
            var cableConnected = step.connectedCables[i];
            cableIdsWithSteps.Add(cableConnected.cableId);
            var cable = _idToCable[cableConnected.cableId];
            _onCableConnected[cableConnected.cableId] = () =>
            {
                Debug.Log($"Callback for cable wit id {cableConnected.cableId}");

                var isCorrect =
                    CheckCableConnectedToRightSocket(cable.GetIdSocketConected(),
                        cableConnected.connected ? cableConnected.connectedSocketId : -1);
                if (!isCorrect)
                {
                    OnFailedStep();
                    return;
                }

                _amountCorrectStatus += 1;
                CheckStepCompleted(step);
            };
            _onCableDisconnected[cableConnected.cableId] = () =>
            {
                var isCorrect =
                    CheckCableConnectedToRightSocket(cable.GetIdSocketConected(),
                        cableConnected.connected ? cableConnected.connectedSocketId : -1);
                if (!isCorrect)
                {
                    OnFailedStep();
                    return;
                }

                _amountCorrectStatus += 1;
                CheckStepCompleted(step);
            };

            cable.OnConnectedCableToSocket += _onCableConnected[cableConnected.cableId];
            cable.OnDisconnectedCableFromSocket += _onCableDisconnected[cableConnected.cableId];
        }

        foreach (var cableId in _idToCable.Keys)
        {
            if (cableIdsWithSteps.Contains(cableId))
            {
                continue;
            }

            var cableWithoutStep = _idToCable[cableId];

            _onCableConnected[cableId] = OnFailedStep;
            _onCableDisconnected[cableId] = OnFailedStep;

            cableWithoutStep.OnConnectedCableToSocket += _onCableConnected[cableId];
            cableWithoutStep.OnDisconnectedCableFromSocket += _onCableDisconnected[cableId];
        }
    }


    bool CheckCorrectStatus(bool desiredStatus, bool status)
    {
        return desiredStatus == status;
    }

    private void UnhookLever()
    {
        if (_onLeverActivated != null)
        {
            lever.OnActivaded -= _onLeverActivated;
        }

        if (_onLeverDeactivated != null)
        {
            lever.OnDeactivaded -= _onLeverDeactivated;
        }
    }

    private void UnhookButton()
    {
        if (_onButtonPressed != null)
        {
            button.OnPresssed -= _onButtonPressed;
        }

        if (_onButtonReleased != null)
        {
            button.OnReleased -= _onButtonReleased;
        }
    }

    bool CheckCableConnectedToRightSocket(int idExpectedSocket, int actualSocket)
    {
        return idExpectedSocket == actualSocket;
    }


    private void UnhookEverything()
    {
        UnhookLever();
        UnhookButton();
        UnhookCables();
    }

    private void UnhookCables()
    {
        foreach (var cableId in _idToCable.Keys)
        {
            var cable = _idToCable[cableId];
            var onConnected = _onCableConnected[cableId];
            var onDisconnected = _onCableDisconnected[cableId];
            if (onConnected != null)
            {
                cable.OnConnectedCableToSocket -= _onCableConnected[cableId];
                _onCableConnected[cableId] = null;
            }

            if (onDisconnected != null)
            {
                cable.OnDisconnectedCableFromSocket -= _onCableDisconnected[cableId];
                _onCableDisconnected[cableId] = null;
            }
        }
    }

    private void OnFailedStep()
    {
        TotalErrors += 1;
        UnhookEverything();
        audioSource.PlayOneShot(failedAudioClip);
        foreach (var success in lightSuccess)
        {
            success.TurnOnOffLight(false);
        }

        _amountCorrectStatus = 0;
        _currStep = 0;
        _isInStepSequence = false;
    }


    private bool CheckStepCompleted(ConsoleSystemStep step)
    {
        Debug.Log(
            $"Amount substeps completed {_amountCorrectStatus}, amount that must be completed {2 + step.connectedCables.Length}");


        bool stepCompleted = _amountCorrectStatus >= 2 + step.connectedCables.Length;
        if (stepCompleted && !_currStepCompleted)
        {
            _currStepCompleted = true;
            onStepCompleted?.Invoke();
        }

        return stepCompleted;
    }

    private void OnStepCompleted()
    {
        Debug.Log($"Unhookimg everything of the succesful step {_currStep}");

        UnhookEverything();
        button.SeeHandlers();

        audioSource.PlayOneShot(successAudioClip);
        lightSuccess[_currStep].TurnOnOffLight(true);
        _currStep += 1;

        if (_currStep >= consoleSystemSteps.Length)
        {
            eventCompleted?.Invoke();
            TotalTime = Time.time - _startTime;
            _isCompleted = true;
            UnhookEverything();
            return;
        }


        StartStep(consoleSystemSteps[_currStep]);
    }

    private void StartStepSequence()
    {
        if (!_isInStepSequence && canStartEvent && !_isCompleted)
        {
            if (_firstTry)
            {
                _startTime = Time.time;
                _firstTry = false;
            }

            audioSource.PlayOneShot(startAudioClip);
            StartStep(consoleSystemSteps[_currStep]);
            _isInStepSequence = true;
        }
    }
}