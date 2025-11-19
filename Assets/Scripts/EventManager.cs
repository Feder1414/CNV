using System;
using System.Collections;
using UnityEngine;
using System.Text;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static EventManager instance;


    public EventSubmarine[] submarineEvents;


    [SerializeField] private int startingEventId;
    [SerializeField] private int amountEvents;
    private CsvManager _csvManager;


    public NPC npc;

    public int currEventId = 0;

    private Guid _userId;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        submarineEvents = new EventSubmarine[amountEvents];

        currEventId = startingEventId;
    }

    private void Start()
    {
    }


    public void StartEvents()
    {
        _userId = Guid.NewGuid();
        StartEvent(startingEventId);
    }

    public void StartEvent(int eventId)
    {
        if (eventId >= submarineEvents.Length)
        {
            //pa luego
        }
        else
        {
            Debug.Log($"Starting event {eventId}");
            submarineEvents[eventId].canStartEvent = true;
            var dialogues = npc.GetDialogLine(eventId);
            if (dialogues != null) DialogueManager.instance.StartDialogue(dialogues, npc);

            submarineEvents[eventId].eventCompleted += ContinueNextEvent;
        }
    }

    public void RepeatDialogue()
    {
        DialogueManager.instance.RepeatDialogue(npc.GetDialogLine(currEventId), npc);
    }

    public void ContinueNextEvent()
    {
        WriteToCsv(submarineEvents[currEventId].TotalTime, submarineEvents[currEventId].TotalErrors);
        currEventId += 1;
        StartEvent(currEventId);
    }

    private void WriteToCsv(float totalTime, int totalErrors)
    {
        _csvManager.WriteTimeErrors(this._userId, totalErrors, totalTime, currEventId,
            (int)GameManager._instance.GetCondition());
    }

    public void RegisterEvent(int eventId, EventSubmarine eventSubmarine)
    {
        if (submarineEvents[eventId] != null)
        {
            throw new Exception("An event has already been registered with the id " + eventId);
        }

        submarineEvents[eventId] = eventSubmarine;
        Debug.Log($"Registering event {eventId}");
    }


    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pageUpKey.wasPressedThisFrame)
        {
            if (currEventId < submarineEvents.Length)
            {
                WriteToCsv(submarineEvents[currEventId].TotalTime, submarineEvents[currEventId].TotalErrors);
            }
        }
    }

    public void RegisterCsvManager(CsvManager csvManager)
    {
        _csvManager = csvManager;
    }
}