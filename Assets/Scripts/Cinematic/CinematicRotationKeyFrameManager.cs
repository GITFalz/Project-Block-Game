using UnityEngine;

public class CinematicRotationKeyFrameManager : MonoBehaviour
{
    public int index;
    public Quaternion rotation;
    public float pTime;

    public void KeyFrameSettings()
    {
        CinematicTimelineManager.SelectedRotationKeyFrame = this;
        CinematicTimelineManager.NewRotateKeyFrameSelected = true;
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
        return 1;
    }
}