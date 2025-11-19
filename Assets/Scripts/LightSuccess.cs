using System;
using UnityEngine;

public class LightSuccess : MonoBehaviour
{
    private Light _pointLight;

    private void Awake()
    {
        _pointLight = GetComponentInChildren<Light>(true);

        if (!_pointLight)
        {
            throw new NullReferenceException("_pointLight is null");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TurnOnOffLight(bool on = true)
    {
        _pointLight.enabled = on;
    }
}