using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LitBulb : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ElectricLever electricLever;
    
    private GameObject _pointLight;
    
    [SerializeField] float maxIntensity;
    [SerializeField] float minIntensity;


    private void Awake()
    {
        _pointLight = GetComponentInChildren<Light>(true)?.gameObject;

        if (!_pointLight)
        {
            throw new NullReferenceException("_pointLight is null");
        }
    }

    void Start()
    {
        electricLever.OnActivaded += OnActivated;
        electricLever.OnDeactivaded += OnDeactivated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnActivated()
    {
        _pointLight.SetActive(true);
    }

    void OnDeactivated()
    {
        _pointLight.SetActive(false);
    }


}
