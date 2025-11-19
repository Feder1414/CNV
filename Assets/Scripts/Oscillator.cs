using System;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public double frequency { get; set; }
    private double increment;
    private double phase;
    private double sampling_frequency = 48000.0;

    private float _gain;
    public float volume = 0.1f;

    public float Volume
    {
        get => volume;
        set => volume = value;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            data[i] = (float)(_gain * Mathf.Sin((float)phase));

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            if (phase > (Mathf.PI * 2))
            {
                phase = 0.0f;
            }
        }
    }

    public void TurnOnOff(bool turnOn)
    {
        if (turnOn)
        {
            _gain = volume;
        }
        else
        {
            _gain = 0.0f;
        }
    }
}