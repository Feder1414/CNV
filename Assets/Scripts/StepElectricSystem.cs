using UnityEngine;

[CreateAssetMenu(menuName = "EventSteps/ElectricSystem")]
public class StepElectricSystem : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LeverTime[] upLevers;
    public int[] downLevers;
    public float duration;
    public float graceTime;
}

[System.Serializable]
public class LeverTime
{
    public int leverId;
    public float leverTime;
}