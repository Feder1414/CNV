using System.Collections.Generic;
using UnityEngine;
using System;

public class NavigationSystemController : MonoBehaviour, EventSubmarine
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] List<Compass> _compasses = new List<Compass>();
    [SerializeField] private LightSuccess lightBulb;
    private int _amountCompassesCompleted = 0;

    public int eventId;


    public event Action eventCompleted;


    public int EventId
    {
        get => eventId;
        set => eventId = value;
    }

    public float TotalTime { get; set; }

    public int TotalErrors { get; set; }

    public bool canStartEvent { get; set; }

    private bool _firstTry = true;

    private float _startTime;


    void Start()
    {
        foreach (var compass in _compasses)
        {
            compass.OnCompassCompletedFailed += CompassCompleted;
            compass.OnCompassChanged += OnCompassChanged;
        }

        EventManager.instance.RegisterEvent(eventId, this);
    }


    // Update is called once per frame

    void CompassCompleted(bool isCompleted)
    {
        if (!canStartEvent)
        {
            return;
        }

        if (isCompleted)
        {
            _amountCompassesCompleted++;
        }
        else
        {
            TotalErrors += 1;
            _amountCompassesCompleted--;
        }

        Debug.Log($"Amount: {_amountCompassesCompleted}");

        if (_amountCompassesCompleted == _compasses.Count && _compasses.Count > 0)
        {
            EventCompleted();

            Debug.Log("Event compasses completed");
            lightBulb.TurnOnOffLight(true);
        }
        else
        {
            lightBulb.TurnOnOffLight(false);
        }
    }

    void OnCompassChanged(float t)
    {
        if (canStartEvent && _firstTry)
        {
            _firstTry = false;
            _startTime = Time.time;

            foreach (var compass in _compasses)
            {
                compass.OnCompassChanged -= OnCompassChanged;
            }
        }
    }

    void EventCompleted()
    {
        TotalTime += Time.time - _startTime;
        eventCompleted?.Invoke();
        foreach (var compass in _compasses)
        {
            compass.OnCompassCompletedFailed -= CompassCompleted;
        }

        Debug.Log("Event completed");
    }
}