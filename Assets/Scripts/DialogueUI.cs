using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class DialogueUI : MonoBehaviour
{
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] Button repeatDialogueButton;


    void Start()
    {
        DialogueManager.instance.dialogueUI = this;
    }

    public void SetDialogueText(string text)
    {
        this.dialogueText.text = text;
    }

    public void SetSpeakerText(string text)
    {
        this.speakerText.text = text;
    }

    public void TurnOnRepeatDialogueButton(bool active = true)
    {
        repeatDialogueButton.GameObject().SetActive(active);
    }
}