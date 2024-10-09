using UnityEngine;

public static class Mathp
{
    public static int sign(float value)
    {
        return value >= 0.0f ? 1 : -1;
    }
    
    public static float Pt(float pin, float pout, float t)
    {
        return pin + t * (pout - pin);
    }

    //tn = (n - Pin) / (Pout - Pin)
    public static float t(float pin, float pout, float s1, float s2)
    {
        float t = same(pin, pout, s1) ? (s1 - pin) / (pout - pin) : (s2 - pin) / (pout - pin);
        return float.IsNaN(t) ? 0.0f : t;
    }

    public static bool same(float pin, float pout, float side)
    {
        return sign(side - pin) == sign(pout - pin);
    }
    
    public static Vector3Int floor(Vector3 a)
    {
        return new ((int)Mathf.Floor(a.x), (int)Mathf.Floor(a.y), (int)Mathf.Floor(a.z));
    }

    public static int abs(float a)
    {
        return (int)Mathf.Abs(a);
    }

    public static float small(params float[] values)
    {
        float smallest = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] < smallest) smallest = values[i];
        }
        
        return smallest;
    }
}