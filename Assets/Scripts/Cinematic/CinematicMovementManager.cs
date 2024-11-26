using System;
using System.Collections.Generic;
using UnityEngine;

public class CinematicMovementManager : MonoBehaviour
{
    [Header("Cinematic Settings")]
    public float smoothing = 1f;
    public float currentTime = 0f;
    public int currentSegment = 0;  
    public float currentAngleTime = 0f;
    public int currentAngleSegment = 0;  
    
    [Header("Cinematic Data")]
    public List<Transform> points; 
    public List<float> times;
    public List<Quaternion> angles;
    public List<float> angleTimes;
    
    [Header("Player")]
    public Rigidbody playerRigidbody;
    public PlayerRotationManager playerRotationManager;
    
    [HideInInspector]
    public bool isPlaying = false;
    
    private List<Mesh> meshes;

    private void Start()
    {
        meshes = new List<Mesh>();
        points = new List<Transform>();
        times = new List<float>();
        angles = new List<Quaternion>();
        angleTimes = new List<float>();
    }
    
    public void Init(List<CinematicPositionKeyFrameManager> keyFrames, List<CinematicRotationKeyFrameManager> rotationKeyFrames)
    {
        points.Clear();
        times.Clear();
        angles.Clear();
        angleTimes.Clear();
        
        foreach (var keyFrame in keyFrames)
        {
            points.Add(keyFrame.keyFrame.transform);
            times.Add(keyFrame.pTime);
        }
        
        foreach (var keyFrame in rotationKeyFrames)
        {
            angles.Add(keyFrame.rotation);
            angleTimes.Add(keyFrame.pTime);
        }
    }

    public void FixedUpdate()
    {
        if (isPlaying)
        {
            Move();
        }
    }

    public void Move()
    {
        if (currentSegment < times.Count)
        {
            currentTime += Time.fixedDeltaTime;
            float segmentTime = times[currentSegment];

            if (currentTime < segmentTime)
            {
                float t = currentTime / segmentTime;

                Vector3 position = CinematicUtils.CatmullRomInterpolation(
                    GetPoint(currentSegment - 1),
                    GetPoint(currentSegment),
                    GetPoint(currentSegment + 1),
                    GetPoint(currentSegment + 2),
                    t,
                    smoothing
                );
                
                playerRigidbody.MovePosition(position);
            }
            else
            {
                currentSegment++;
                currentTime = 0f;
            }
        }
        
        if (currentAngleSegment < angleTimes.Count)
        {
            currentAngleTime += Time.fixedDeltaTime;
            float segmentTime = angleTimes[currentAngleSegment];

            if (currentAngleTime < segmentTime)
            {
                float t = currentAngleTime / segmentTime;

                Quaternion lastRotation = GetRotation(currentAngleSegment - 1);
                Quaternion currentRotation = GetRotation(currentAngleSegment);
                Quaternion nextRotation = GetRotation(currentAngleSegment + 1);

                Quaternion q1 = Quaternion.Slerp(lastRotation, currentRotation, t);
                Quaternion q2 = Quaternion.Slerp(currentRotation, nextRotation, t);

                Quaternion rotation = Quaternion.Slerp(q1, q2, smoothing);

                playerRotationManager.transform.rotation = rotation;
            }
            else
            {
                currentAngleSegment++;
                currentAngleTime = 0f;
            }
        }
    }

    public void Reset() 
    {
        currentTime = 0f;
        currentSegment = 0;
        currentAngleTime = 0f;
        currentAngleSegment = 0;
    }

    public void Hide()
    {
        foreach (var point in points)
        {
            meshes.Add(point.gameObject.GetComponent<MeshFilter>().mesh);
            point.gameObject.GetComponent<MeshFilter>().mesh = null;
        }
    }
    
    public void Show()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].gameObject.GetComponent<MeshFilter>().mesh = meshes[i];
        }
    }
    
    private Vector3 GetPoint(int index)
    {
        if (index < 0) return points[0].position;
        if (index >= points.Count) return points[points.Count - 1].position;
        return points[index].position;
    }
    
    // Get the rotation for a given index
    private Quaternion GetRotation(int index)
    {
        if (index < 0) return angles[0];
        if (index >= angles.Count) return angles[angles.Count - 1];
        return angles[index];
    }
}