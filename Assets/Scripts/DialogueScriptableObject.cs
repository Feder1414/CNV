using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Sequence")]
public class DialoguesScriptableObject : ScriptableObject
{
    public int eventId;
    public DialogueLine[] lines;
    public int repeatStartIndex;
    public int repeatEndIndex;
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea] public string text;
    public AudioClip[] audioClip;
    public string animTrigger;
    public float typeSpeed;
    public float autoAdvanceAfter = 0;
    public bool autoCalculateTypeSpeedAndDuration = false;
    private float extendedDuration = 0.0f;
}