using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "EventSteps/ConsoleSystem")]
public class ConsoleSystemStep : ScriptableObject
{
    public string leverId;
    public bool leverActivated;
    public float leverTime;

    public string buttonId;
    public bool buttonActivated;
    public float buttonTime;

    public float compassId;
    public float amountDegreesToRotate;

    public CableToSocket[] connectedCables;
}

[System.Serializable]
public class CableToSocket
{
    [FormerlySerializedAs("ropeId")] public int cableId;
    public bool connected;
    public int connectedSocketId;
}