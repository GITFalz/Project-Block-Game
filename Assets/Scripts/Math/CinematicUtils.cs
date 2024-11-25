using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CinematicUtils
{
    public static Vector3 CatmullRomInterpolation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, float smoothFactor)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        
        Vector3 result = 0.5f *
                         ((2f * p1) +
                          (-p0 + p2) * t +
                          (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                          (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        
        return Vector3.Lerp(Vector3.Lerp(p1, p2, t), result, smoothFactor);
    }
    
    public static Quaternion Angle2Quat(float azimuth, float elevation, float roll)
    {
        return Quaternion.AngleAxis(azimuth, Vector3.forward) * 
               Quaternion.AngleAxis(elevation, Vector3.right) * 
               Quaternion.AngleAxis(roll, Vector3.up);
    }
    
    public static List<Quaternion> CreateClosedCurve(List<Quaternion> rotations)
    {
        rotations = Canonicalized(rotations.Concat(rotations.Take(2)).ToList());
        List<Quaternion> controlPoints = new List<Quaternion>();

        for (int i = 0; i < rotations.Count - 2; i++)
        {
            Quaternion q_1 = rotations[i];
            Quaternion q0 = rotations[i + 1];
            Quaternion q1 = rotations[i + 2];

            Quaternion qOffset = Offset(q_1, q0, q1);
            controlPoints.Add(Quaternion.Slerp(Quaternion.identity, q0, -1f / 3f) * qOffset);
            controlPoints.Add(q0);
            controlPoints.Add(q0);
            controlPoints.Add(Quaternion.Slerp(Quaternion.identity, q0, 1f / 3f) * qOffset);
        }

        controlPoints = controlPoints.Skip(controlPoints.Count - 2).Concat(controlPoints.Take(controlPoints.Count - 2)).ToList();
        List<List<Quaternion>> segments = new List<List<Quaternion>>();

        for (int i = 0; i < controlPoints.Count; i += 4)
        {
            segments.Add(controlPoints.Skip(i).Take(4).ToList());
        }

        return DeCasteljau(segments);
    }

    private static List<Quaternion> Canonicalized(List<Quaternion> rotations)
    {
        // Implement the canonicalization logic here
        return rotations;
    }

    private static Quaternion Offset(Quaternion q_1, Quaternion q0, Quaternion q1)
    {
        // Implement the offset calculation logic here
        return Quaternion.identity;
    }

    private static List<Quaternion> DeCasteljau(List<List<Quaternion>> segments)
    {
        // Implement the De Casteljau algorithm here
        return new List<Quaternion>();
    }
}