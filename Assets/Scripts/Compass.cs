using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class Compass : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private XRKnob _xrKnob;
    [SerializeField] private float lowerAngleBound;
    [SerializeField] private float upperAngleBound;
    [SerializeField] private float fallOffDegrees;
    [SerializeField] private float pitchMin;

    [SerializeField] private float pitchMax;

    //[SerializeField] private AudioSource audioSource;
    [SerializeField] private Oscillator _oscillator;


    private float _valueT;

    public float MinAngle { get; private set; }
    public float MaxAngle { get; private set; }

    public event Action<bool> OnCompassCompletedFailed;

    public event Action<float> OnCompassChanged;

    private bool _isCompleted = false;


    void Awake()
    {
        _xrKnob = GetComponent<XRKnob>();
        if (!_xrKnob)
        {
            throw new Exception("_xrKnob not found");
        }

        if (!((lowerAngleBound >= _xrKnob.minAngle) &&
              (lowerAngleBound <= _xrKnob.maxAngle)))
        {
            throw new Exception("LowerAngleBound must be between lowerAngle and upperAngleBound");
        }

        if (!((upperAngleBound >= _xrKnob.minAngle) &&
              (upperAngleBound <= _xrKnob.maxAngle)))
        {
            throw new Exception("UpperAngleBound must be between lowerAngle and upperAngleBound");
        }


        if (!(upperAngleBound >= lowerAngleBound))
        {
            throw new Exception("UpperAngleBound must be greater than lowerAngleBound");
        }

        MinAngle = _xrKnob.minAngle;
        MaxAngle = _xrKnob.maxAngle;

        _xrKnob.onValueChange.AddListener(OnXrKnobChange);

        // if (!audioSource)
        // {
        //     throw new Exception("audioSource not found");
        // }
        //
        // audioSource.loop = true;
        // audioSource.clip = pitch;

        _xrKnob.selectEntered.AddListener(StartPlayingSound);
        _xrKnob.selectExited.AddListener(StopPlayingSound);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnXrKnobChange(float value)
    {
        _valueT = value;
        CheckCompassInRange();
        Debug.Log("XrKnob changed");
    }

    public float GetValueT()
    {
        return _valueT;
    }

    void CheckCompassInRange()
    {
        float currentAngle = Mathf.Lerp(MinAngle, MaxAngle, GetValueT());

        float d = GetDistanceRange(currentAngle);

        Debug.Log($"CurrentAngle: {currentAngle}");

        Debug.Log($"Distance: {d}");

        float t = 1f - Mathf.Clamp01(d / Mathf.Max(0.0001f, fallOffDegrees));

        float soundPitch = Mathf.Lerp(pitchMin, pitchMax, t);

        Debug.Log($"Sound Pitch: {soundPitch} ");

        _oscillator.frequency = soundPitch;

        OnCompassChanged?.Invoke(GetValueT());


        if (currentAngle > lowerAngleBound && currentAngle < upperAngleBound && !_isCompleted)
        {
            _isCompleted = true;
            OnCompassCompletedFailed?.Invoke(true);
        }
        else if ((currentAngle > upperAngleBound || currentAngle < lowerAngleBound) && _isCompleted)
        {
            _isCompleted = false;
            OnCompassCompletedFailed?.Invoke(false);
        }
    }

    float GetDistanceRange(float currentAngle)
    {
        if (currentAngle > upperAngleBound)
        {
            return Mathf.Abs(currentAngle - upperAngleBound);
        }

        if (currentAngle < lowerAngleBound)
        {
            return Mathf.Abs(currentAngle - lowerAngleBound);
        }

        return 0;
    }

    void StartPlayingSound(SelectEnterEventArgs args)
    {
        //audioSource.Play();
        _oscillator.TurnOnOff(true);
        Debug.Log("StartPlayingSound");
    }

    void StopPlayingSound(SelectExitEventArgs args)
    {
        //audioSource.Stop();
        _oscillator.TurnOnOff(false);
        Debug.Log("StopPlayingSound");
    }
}