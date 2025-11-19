using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Dictionary<int, DialoguesScriptableObject> eventToDialogueLine =
        new Dictionary<int, DialoguesScriptableObject>();

    public DialoguesScriptableObject[] dialogues;

    private Animator animator;

    private void Awake()
    {
        foreach (DialoguesScriptableObject dialog in dialogues)
        {
            eventToDialogueLine[dialog.eventId] = dialog;
        }

        animator = GetComponentInChildren<Animator>();

        if (!animator)
        {
            throw new SystemException("NPC has no animator attached in children");
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.instance.npc = this;
    }


    // Update is called once per frame
    void Update()
    {
    }

    public DialoguesScriptableObject GetDialogLine(int eventId)
    {
        if (eventToDialogueLine.ContainsKey(eventId))
        {
            return eventToDialogueLine[eventId];
        }

        return null;
    }

    public void TriggerAnimation(string triggerAnimation)
    {
        animator.SetTrigger(triggerAnimation);
    }
}