using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameManager _instance;

    [SerializeField] Condition _condition;


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame &&
            SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartConditionOne();
        }
        else if (Keyboard.current != null && Keyboard.current.downArrowKey.wasPressedThisFrame &&
                 SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartConditionTwo();
        }
    }

    // Update is called once per frame
    public void StartConditionOne()
    {
        _condition = Condition.Gesture;
        SceneManager.LoadScene(1);
    }

    public void StartConditionTwo()
    {
        _condition = Condition.GestureLess;
        SceneManager.LoadScene(1);
    }

    public Condition GetCondition()
    {
        return _condition;
    }
}

public enum Condition
{
    Gesture,
    GestureLess
}