using System;
using System.Collections;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Composites;

public class PressureSystemController : MonoBehaviour, EventSubmarine
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform needle;
    public float maxAngle = 259;
    public float minAngle = 0;

    public bool seeNeedleAngle = false;
    public bool buttonPresed = false;
    [SerializeField] private float timeButtonPressed = 3.0f;
    [SerializeField] private float timeToReturn = 3.5f;


    [SerializeField] RangeTime rangeRed;
    [SerializeField] private RangeTime rangeGreen;
    private float currAngle = 0;

    private bool firstCheck = false;
    private bool secondCheck = false;

    private bool isCheckingFirst = false;

    private bool isCheckingSecond = false;

    private bool completed = false;

    public event Action eventCompleted;

    public int eventId;

    private bool _isCompleted = false;

    [SerializeField] private PressButton pressButton;

    [SerializeField] private LightSuccess lightFirstStep;
    [SerializeField] private LightSuccess lightSecondStep;

    [SerializeField] private AudioClip soundCorrectStep;
    [SerializeField] private AudioClip soundFailedStep;

    [SerializeField] private AudioSource audioSource;

    private bool _firstTry = true;


    public int EventId
    {
        get => eventId;
        set => eventId = value;
    }

    public float TotalTime { get; set; }

    public int TotalErrors { get; set; }

    public bool canStartEvent { get; set; }

    private float _startTime;


    private Quaternion _baseLocalRotNeedle;

    void Awake()
    {
        _baseLocalRotNeedle = needle.localRotation;
        if (!pressButton)
        {
            throw new SystemException("The pressure system does not have a botton");
        }

        pressButton.OnPresssed += () =>
        {
            buttonPresed = true;
            StartStepSequence();
        };
        pressButton.OnReleased += () => { buttonPresed = false; };
    }

    void Start()
    {
        EventManager.instance.RegisterEvent(eventId, this);
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void OnSteerWheelChange(float angleChange)
    {
        Debug.Log("angleChange = " + angleChange);
        this.currAngle = Mathf.Lerp(minAngle, maxAngle, angleChange);

        needle.localRotation = _baseLocalRotNeedle * Quaternion.AngleAxis(this.currAngle, Vector3.forward);

        Debug.Log(angleChange);

        if (seeNeedleAngle)
        {
            Debug.Log($"Current angle needle {currAngle}");
        }

        StartStepSequence();
    }

    private void StartStepSequence()
    {
        if (rangeRed.rangeFloat.IsInRange(this.currAngle) && !firstCheck && canStartEvent && !_isCompleted &&
            pressButton.IsPressed())
        {
            Debug.Log("Starting event");

            if (_firstTry)
            {
                TotalTime = Time.time - _startTime;
                _firstTry = false;
            }

            StartCoroutine(CheckRanges());
        }
    }

    IEnumerator CheckRanges()
    {
        // Chequear primero los 3 segundos
        if (!isCheckingFirst && !completed)
        {
            isCheckingFirst = true;
        }
        else
        {
            yield break;
        }


        float startTime = Time.time;

        float currTime = startTime;

        while (currTime < startTime + timeButtonPressed)
        {
            if (!buttonPresed || !rangeRed.rangeFloat.IsInRange(currAngle))
            {
                isCheckingFirst = false;
                FailedStep();

                yield break;
            }

            yield return null;
            currTime = Time.time;
        }

        firstCheck = true;


        audioSource.PlayOneShot(soundCorrectStep);
        lightFirstStep.TurnOnOffLight(true);

        yield return CheckGreenRange();

        firstCheck = false;

        isCheckingFirst = false;
    }

    IEnumerator CheckGreenRange()
    {
        if (!isCheckingSecond)
        {
            isCheckingSecond = true;
        }
        else
        {
            yield break;
        }

        float startTime = Time.time;
        float currTime = startTime;
        while (currTime < startTime + timeToReturn)
        {
            if (!buttonPresed)
            {
                isCheckingSecond = false;
                FailedStep();
                yield break;
            }

            yield return null;
            currTime = Time.time;
        }

        if (!rangeGreen.rangeFloat.IsInRange(this.currAngle) || !buttonPresed)
        {
            isCheckingSecond = false;
            FailedStep();
            yield break;
        }

        lightSecondStep.TurnOnOffLight(true);
        audioSource.PlayOneShot(soundCorrectStep);

        completed = true;
        eventCompleted?.Invoke();


        isCheckingSecond = false;
    }

    private void FailedStep()
    {
        TotalErrors += 1;
        audioSource.PlayOneShot(soundFailedStep);
        lightFirstStep.TurnOnOffLight(false);
        lightSecondStep.TurnOnOffLight(false);
    }
}


public interface EventSubmarine
{
    public event Action eventCompleted;
    public int EventId { get; set; }

    public float TotalTime { get; set; }

    public int TotalErrors { get; set; }

    public bool canStartEvent { get; set; }
}