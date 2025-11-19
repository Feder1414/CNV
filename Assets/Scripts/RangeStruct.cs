using UnityEngine;
using UnityEngine;

[System.Serializable]
public struct RangeFloat
{
    public float min;
    public float max;

    public RangeFloat(float min, float max)
    {
        this.min = min;
        this.max = max;
        if (this.min > this.max) (this.min, this.max) = (this.max, this.min);
    }

    public bool IsInRange(float number)
    {
        return number >= this.min && number <= this.max;
    }
}

[System.Serializable]
public struct RangeTime
{
    public RangeFloat rangeFloat;
    public float time;

    public RangeTime(RangeFloat rangeFloat, float time)
    {
        this.rangeFloat = rangeFloat;
        this.time = time;
    }
}