using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CinematicKeyFrameSettingsManager : MonoBehaviour
{
    public Vector3 position;
    public float pTime;
    
    public Vector3 rotation;
    public float rTime;
    
    public Vector3CustomUI positionCustomUI;
    public ValueCustomUI pTimeCustomUI;
    public ValueCustomUI rTimeCustomUI;
    
    [Header("Timeline")]
    public CinematicTimelineManager timelineManager;
    
    public CinematicPositionKeyFrameManager positionKeyFrameManager;
    public CinematicRotationKeyFrameManager rotationKeyFrameManager;
    

    public static bool swipping;
    public static TMP_InputField swippingInputField;

    public static bool moving;
    public static CinematicPositionKeyFrameManager movingObject;
    
    private Func<bool> leftClick;
    private Func<bool> c;
    private bool holdingC;

    private void Start()
    {
        leftClick = PlayerInput.Instance.LeftClickInput;
        c = PlayerInput.Instance.CInput;
    }

    private void Update()
    {
        if (positionKeyFrameManager)
        {
            if (positionCustomUI.valueChanged)
            {
                positionCustomUI.valueChanged = false;
                position = positionCustomUI.GetVector3();
                positionKeyFrameManager.UpdatePosition(position);
            }

            if (pTimeCustomUI.valueChanged)
            {
                pTimeCustomUI.valueChanged = false;
                pTime = float.Parse(pTimeCustomUI.GetValue(), CultureInfo.InvariantCulture);
                positionKeyFrameManager.UpdateTime(pTime);
            }
        }
        
        if (rotationKeyFrameManager)
        {
            if (rTimeCustomUI.valueChanged)
            {
                rTimeCustomUI.valueChanged = false;
                rTime = float.Parse(rTimeCustomUI.GetValue(), CultureInfo.InvariantCulture);
                rotationKeyFrameManager.UpdateTime(rTime);
            }
        }
        
        if (swipping)
        {
            float x = Input.GetAxis("Mouse X");
            if (swippingInputField)
            {
                float value = float.Parse(swippingInputField.text, CultureInfo.InvariantCulture) + x;
                swippingInputField.text = value.ToString(CultureInfo.InvariantCulture);
            }
        }
        
        if (moving)
        {
            float x = Input.GetAxis("Mouse X");
            if (movingObject && movingObject.index > 0)
            {
                if (!c())
                {
                    CinematicPositionKeyFrameManager backPositionKeyFrame = timelineManager.positionKeyframes[movingObject.index - 1];
                    backPositionKeyFrame.UpdateTime(backPositionKeyFrame.pTime + (x / 10));
                    movingObject.transform.position += new Vector3(x, 0, 0);
                }
                else
                {
                    holdingC = true;
                    movingObject.transform.SetParent(timelineManager.oldPositionTimeline);
                    float mouseX = Input.mousePosition.x;
                    float mouseY = Input.mousePosition.y;
                    movingObject.transform.position = new Vector3(mouseX, mouseY, movingObject.transform.position.z);
                }
                
                if (!c() && holdingC)
                {
                    holdingC = false;
                    for (int i = 0; i < timelineManager.positionKeyframes.Count; i++)
                    {
                        if (timelineManager.positionKeyframes[i].index != movingObject.index)
                        {
                            if (timelineManager.positionKeyframes[i].transform.position.x >= movingObject.transform.position.x)
                            {
                                int index = timelineManager.positionKeyframes[i].index;
                                CinematicPositionKeyFrameManager positionKeyFrame = timelineManager.positionKeyframes[movingObject.index];
                                timelineManager.positionKeyframes.RemoveAt(movingObject.index);
                                
                                for (int j = 0; j < timelineManager.positionKeyframes.Count; j++)
                                {
                                    if (timelineManager.positionKeyframes[j].index == index)
                                    {
                                        movingObject.transform.SetParent(timelineManager.positionTimeline);
                                        movingObject.transform.SetSiblingIndex(j);
                                        timelineManager.positionKeyframes.Insert(j, positionKeyFrame);
                                        break;
                                    }
                                }
                                for (int j = 0; j < timelineManager.positionKeyframes.Count; j++)
                                {
                                    timelineManager.positionKeyframes[j].index = j;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void KeyFrameSettings(CinematicPositionKeyFrameManager positionKeyFrame)
    {
        positionKeyFrameManager = positionKeyFrame;
            
        position = positionKeyFrame.position;
        pTime = positionKeyFrame.pTime;
        
        pTimeCustomUI.UpdateUI(pTime.ToString(CultureInfo.InvariantCulture));
        positionCustomUI.UpdateUI(position);
    }
    
    public void KeyFrameRotationSettings(CinematicRotationKeyFrameManager rotationKeyFrame)
    {
        rotationKeyFrameManager = rotationKeyFrame;
        rTime = rotationKeyFrame.pTime;

        rTimeCustomUI.UpdateUI(rTime.ToString(CultureInfo.InvariantCulture));
    }
}