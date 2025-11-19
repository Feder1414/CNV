using System;
using UnityEngine;
using System.Text;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public DialogueUI dialogueUI;


    private AudioSource _audioSource;


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

        _audioSource = GetComponent<AudioSource>();

        if (!_audioSource)
        {
            throw new SystemException("No audio source assigned to dialogue manager");
        }
    }

    public void StartDialogue(DialoguesScriptableObject dialogues, NPC npc)
    {
        StopAllCoroutines();
        dialogueUI.TurnOnRepeatDialogueButton(false);
        _audioSource.Stop();
        StartCoroutine(TriggerDialogue(dialogues, npc));
    }

    public void RepeatDialogue(DialoguesScriptableObject dialogues, NPC npc)
    {
        StopAllCoroutines();
        dialogueUI.TurnOnRepeatDialogueButton(false);
        _audioSource.Stop();
        StartCoroutine(TriggerDialogue(dialogues, npc, false));
    }


    IEnumerator TriggerDialogue(DialoguesScriptableObject dialogues, NPC npc, bool sayEverything = true)
    {
        StringBuilder currText = new StringBuilder();

        for (int indexLine = (sayEverything ? 0 : dialogues.repeatStartIndex);
             indexLine < (sayEverything ? dialogues.lines.Length : dialogues.repeatEndIndex);
             indexLine++)
        {
            var line = dialogues.lines[indexLine];
            if (line.animTrigger.Length > 0 && GameManager._instance &&
                GameManager._instance.GetCondition() == Condition.Gesture)
            {
                npc.TriggerAnimation(line.animTrigger);
            }

            var audioCLip = line.audioClip[0];

            if (audioCLip)
            {
                _audioSource.PlayOneShot(audioCLip);
            }


            if (line.typeSpeed == 0)
            {
                throw new SystemException("Typespeed cannot be 0");
            }


            this.dialogueUI.SetSpeakerText(line.speaker);
            float tBetweenCharacters = 1 / line.typeSpeed;


            for (int i = 0; i < line.text.Length; i++)
            {
                currText.Append(line.text[i]);
                this.dialogueUI.SetDialogueText(currText.ToString());


                yield return new WaitForSeconds(tBetweenCharacters);
            }

            while (_audioSource.isPlaying)
            {
                yield return null;
            }

            currText.Clear();
            yield return new WaitForSeconds(line.autoAdvanceAfter);
        }

        dialogueUI.TurnOnRepeatDialogueButton();
    }
}