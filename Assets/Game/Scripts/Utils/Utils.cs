using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Limit2D
{
    public float min;
    public float max;
}

public class Utils
{
    /// <summary>
    /// Normaliza un valor (value) en valores [minNormalized, maxNormalized] pasandole el rango minRange y maxRange donde puede estar 
    /// </summary>
    public static float normalizeValues(float minNormalized, float maxNormalized, float minRange, float maxRange, float value)
    {
        float normalizedValue = (maxNormalized - minNormalized)*((value-minRange)/(maxRange-minRange))+minNormalized;
        return normalizedValue;
    }

    /// <summary>
    /// Convierte un ángulo de [0, 360] a  [0, -180]
    /// </summary>
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    /// <summary>
    /// Convierte un ángulo de [0, -180] a [0, 360]
    /// </summary>
    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }
}
