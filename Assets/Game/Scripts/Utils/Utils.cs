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

    public static float NormalizeAngle(float a)
    {
        return a - 180f * Mathf.Floor((a + 180f) / 180f);
    }

    static public float AngularClamp(float val, float min, float max)
    {
        if(val < min || val > max)
        {
            // Calculamos a que angulo esta mas cerca
            float deltaToMin = Mathf.Abs(Mathf.DeltaAngle(val, min));
            float deltaToMax = Mathf.Abs(Mathf.DeltaAngle(val, max));
            if(deltaToMin < deltaToMax)
            {
                val = min;
            }
            else
            {
                val = max;
            }
        }

        return val;
    }

    /// <summary>
    /// Devuelve el layermask de la máscara indicada. Mira en la coinfiguracion de fisicas para ver con que tiene que colisionar y con que no
    /// </summary>
    /// <param name="currentLayer"></param>
    /// <returns></returns>
    public static LayerMask GetPhysicsLayerMask(int currentLayer)
    {
        int finalMask = 0;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(currentLayer, i)) finalMask = finalMask | (1 << i);
        }
        return finalMask;
    }
}
