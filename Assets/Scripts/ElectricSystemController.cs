using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class ElectricSystemController : MonoBehaviour, EventSubmarine
{
    [SerializeField] private AudioClip stepCorrect;
    [SerializeField] private AudioClip stepWrong;
    [SerializeField] private AudioClip powerOutage;
    private AudioSource _audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<ElectricLever> levers;

    [SerializeField] private List<StepElectricSystem> steps;

    [SerializeField] private List<LightSuccess> lightBulbs;

    private bool middleCheck = false;

    private Dictionary<int, ElectricLever> idToLever = new Dictionary<int, ElectricLever>();


    [SerializeField] private int startingLeverId = 0;

    private bool _checkingSteps;

    public event Action eventCompleted;

    public int eventId;

    public int EventId
    {
        get => eventId;
        set => eventId = value;
    }

    public float TotalTime { get; set; }

    public int TotalErrors { get; set; }

    public bool canStartEvent { get; set; }

    private bool _isCompleted;

    private bool _firstTry = true;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 0.0f;
        }
    }

    private float _startTime;


    void Start()
    {
        EventManager.instance.RegisterEvent(eventId, this);
        foreach (var lever in levers)
        {
            idToLever[lever.GetLeverId()] = lever;
        }

        if (idToLever.TryGetValue(startingLeverId, out var startLever))
        {
            startLever.OnActivaded += StartEvent;
            Debug.Log("Suscribed to starting leverId");
        }

        if (steps.Count != lightBulbs.Count)
        {
            throw new Exception("Step count must be equal to lightBulbs.Count");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Iniciar el evento
    void StartEvent()
    {
        if (canStartEvent && !_isCompleted)
        {
            if (_firstTry)
            {
                _startTime = Time.time;
                Debug.Log($"StartTime event electric {_startTime} ");
                _firstTry = false;
            }

            Debug.Log("Starting step sequence");
            StartCoroutine(CheckSteps());
        }
    }

    IEnumerator CheckSteps()
    {
        if (_checkingSteps || _isCompleted)
        {
            yield break;
        }

        middleCheck = true;

        for (int i = 0; i < steps.Count; i++)
        {
            Debug.Log("Step #" + i + ": ");
            var step = steps[i];
            foreach (var upLever in step.upLevers)
            {
                StartCoroutine(CheckIsUpDown(upLever.leverId, upLever.leverTime, true));
            }

            foreach (var downLever in step.downLevers)
            {
                StartCoroutine(CheckIsUpDown(downLever, step.duration, false));
            }

            yield return new WaitForSeconds(step.duration);
            _audioSource.PlayOneShot(stepCorrect);
            lightBulbs[i].TurnOnOffLight();
            yield return new WaitForSeconds(step.graceTime);
        }

        TotalTime = Time.time - _startTime;
        eventCompleted?.Invoke();


        _isCompleted = true;
        Debug.Log($"Completed electric system event with id {eventId}");
    }

    IEnumerator CheckIsUpDown(int leverId, float time, bool isUp = true)
    {
        float timer = 0;

        while (timer < time)
        {
            bool leverExist = idToLever.TryGetValue(leverId, out var lever);
            if (!leverExist)
            {
                throw new KeyNotFoundException($"Lever id {leverId} no existe en idToLever.");
            }

            if (isUp)
            {
                if (!lever.IsActive())
                {
                    FailedStep();
                    StopAllCoroutines();
                }
            }
            else
            {
                if (lever.IsActive())
                {
                    FailedStep();
                    StopAllCoroutines();
                }
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    void FailedStep()
    {
        _checkingSteps = false;
        foreach (var light in lightBulbs)
        {
            light.TurnOnOffLight(false);
        }

        _audioSource.PlayOneShot(stepWrong);
        _audioSource.PlayOneShot(powerOutage);

        StopAllCoroutines();
    }
}