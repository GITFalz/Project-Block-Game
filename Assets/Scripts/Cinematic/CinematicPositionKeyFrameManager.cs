using UnityEngine;

public class CinematicPositionKeyFrameManager : MonoBehaviour
{
    public int index;
    public Vector3 position;
    public float pTime;
    public Transform keyFrame;

    public void SetValues(int i, Vector3 pos, Transform kf)
    {
        index = i;
        position = pos;
        keyFrame = kf;
    }

    public void KeyFrameSettings()
    {
        Debug.Log(index);
        
        CinematicTimelineManager.SelectedPositionKeyFrame = this;
        CinematicTimelineManager.NewPosKeyFrameSelected = true;
    }
    
    public void UpdatePosition(Vector3 pos)
    {
        position = pos;
        keyFrame.position = position;
    }
    
    public void UpdateTime(float time)
    {
        pTime = time;
        
        RectTransform keyFrameRect = GetComponent<RectTransform>();
        Vector2 currentSize = keyFrameRect.sizeDelta;
        keyFrameRect.sizeDelta = new Vector2(pTime * 100, currentSize.y);
    }
    
    public int Get()
    {
        return 0;
    }
}