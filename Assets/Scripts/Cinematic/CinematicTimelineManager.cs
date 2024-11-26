using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CinematicTimelineManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public Transform playerRotation;
    
    [Header("In Game")]
    public GameObject keyFramePrefab;
    public Transform keyFrameParent;
    
    [Header("Timeline")]
    public Transform positionTimeline;
    public Transform oldPositionTimeline;
    public GameObject positionKeyframePrefab;
    public Transform timelineBar;
    
    public Transform rotationTimeline;
    public Transform oldRotationTimeline;
    public GameObject rotationKeyframePrefab;
    
    [Header("Scroll View")]
    public GameObject scrollViewport;
    public static ScrollRect scrollRect;
    
    [Header("Keyframes")]
    public CinematicMovementManager cinematicMovementManager;
    
    public List<CinematicPositionKeyFrameManager> positionKeyframes;
    public List<CinematicRotationKeyFrameManager> rotationKeyframes;
    
    public static CinematicPositionKeyFrameManager SelectedPositionKeyFrame;
    public static bool NewPosKeyFrameSelected;
    
    public static CinematicRotationKeyFrameManager SelectedRotationKeyFrame;
    public static bool NewRotateKeyFrameSelected;
    
    public CinematicKeyFrameSettingsManager keyFrameSettingsManager;
    
    private Func<bool> controlInput;
    private Func<bool> c;
    private Func<bool> q;
    private Func<bool> d;
    private Func<bool> p;
    private Func<bool> a;
    private Func<bool> r;
    
    private Switch pSwitch;
    private Switch qSwitch;
    private Switch dSwitch;
    private Switch aSwitch;
    private Switch rSwitch;
    
    private void Start()
    {
        positionKeyframes = new List<CinematicPositionKeyFrameManager>();
        rotationKeyframes = new List<CinematicRotationKeyFrameManager>();
        
        controlInput = PlayerInput.Instance.ControlInput;
        
        c = PlayerInput.Instance.CInput;
        q = PlayerInput.Instance.QInput;
        d = PlayerInput.Instance.DInput;
        p = PlayerInput.Instance.PInput;
        a = PlayerInput.Instance.AInput;
        r = PlayerInput.Instance.RInput;
        
        pSwitch = new Switch(p);
        qSwitch = new Switch(q);
        dSwitch = new Switch(d);
        aSwitch = new Switch(a);
        rSwitch = new Switch(r);
        
        scrollRect = scrollViewport.GetComponent<ScrollRect>();
    }

    private void Update()
    {
        if (controlInput())
        {
            if (c())
            {
                if (pSwitch.CanSwitch())
                {
                    GameObject keyframe = Instantiate(keyFramePrefab, player);
                    keyframe.transform.SetParent(keyFrameParent);
                    keyframe.transform.position = player.position;
                    
                    GameObject positionKeyframe = Instantiate(positionKeyframePrefab, positionTimeline);
                    
                    CinematicPositionKeyFrameManager positionKeyFrameManager = positionKeyframe.GetComponent<CinematicPositionKeyFrameManager>();
                    positionKeyFrameManager.index = positionKeyframes.Count;
                    positionKeyFrameManager.position = player.position;
                    positionKeyFrameManager.keyFrame = keyframe.transform;
                    positionKeyFrameManager.pTime = 2;
                    
                    RectTransform keyFrameRect = positionKeyframe.GetComponent<RectTransform>();
                    Vector2 currentSize = keyFrameRect.sizeDelta;
                    keyFrameRect.sizeDelta = new Vector2(200, currentSize.y);
                    
                    positionKeyframes.Add(positionKeyFrameManager);
                }
                
                if (aSwitch.CanSwitch())
                {
                    GameObject positionKeyframe = Instantiate(rotationKeyframePrefab, rotationTimeline);
                    
                    CinematicRotationKeyFrameManager rotationKeyFrameManager = positionKeyframe.GetComponent<CinematicRotationKeyFrameManager>();
                    rotationKeyFrameManager.index = rotationKeyframes.Count;
                    rotationKeyFrameManager.rotation = playerRotation.rotation;
                    rotationKeyFrameManager.pTime = 2;
                    
                    RectTransform keyFrameRect = positionKeyframe.GetComponent<RectTransform>();
                    Vector2 currentSize = keyFrameRect.sizeDelta;
                    keyFrameRect.sizeDelta = new Vector2(200, currentSize.y);
                    
                    rotationKeyframes.Add(rotationKeyFrameManager);
                }
                
                if (qSwitch.CanSwitch())
                {
                    Clear();
                }
                
                if (dSwitch.CanSwitch())
                {
                    
                }
            }
        }

        if (NewPosKeyFrameSelected)
        {
            UpdateKeyFrameSettings();
            NewPosKeyFrameSelected = false;
        }
        
        if (NewRotateKeyFrameSelected)
        {
            UpdateKeyFrameRotateSettings();
            NewRotateKeyFrameSelected = false;
        }
    }

    public void Clear()
    {
        foreach (Transform child in keyFrameParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        positionKeyframes.Clear();
        rotationKeyframes.Clear();
        
        foreach (Transform child in positionTimeline.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in rotationTimeline.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Action()
    {
        cinematicMovementManager.Init(positionKeyframes, rotationKeyframes);
    }

    public void MoveTimelineBar()
    {
        Vector3 mousePos = Input.mousePosition;
        timelineBar.position = new Vector3(mousePos.x, timelineBar.position.y, timelineBar.position.z);
    }
    
    public void UpdateKeyFrameSettings()
    {
        keyFrameSettingsManager.KeyFrameSettings(SelectedPositionKeyFrame);
    }
    
    public void UpdateKeyFrameRotateSettings()
    {
        keyFrameSettingsManager.KeyFrameRotationSettings(SelectedRotationKeyFrame);
    }
}