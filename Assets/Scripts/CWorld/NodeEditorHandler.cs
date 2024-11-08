using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeEditorHandler : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public GameObject sampleNodePrefab;
    public GameObject panel;

    public List<ParameterObject> parameters = new List<ParameterObject>();
    

    void Update()
    {
        if (IsMouseOverSpecificUIElement("NodeEditor"))
        {
            Debug.Log("Mouse is over the specific UI element.");
        }
    }

    public void AddNode(GameObject nodePrefab)
    {
        Instantiate(nodePrefab, panel.transform);
    }
    
    public void Toggle(GameObject p)
    {
        p.SetActive(!p.activeSelf);
    }
    
    private bool IsMouseOverSpecificUIElement(string tag)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class ParameterObject
{
    public string name;
    public ParameterAbstract parameter;
}

public abstract class ParameterAbstract : ScriptableObject
{
    public abstract bool GetValue(out string value);
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Node/Parameter/1Float")]
public class Parameter1Float : ParameterAbstract
{
    public string name;
    public TMP_InputField a;

    public override bool GetValue(out string value)
    {
        value = $"{name} ";
        if (!float.TryParse(a.text, out var result)) return false;
        value = $"{result}";
        return true;
    }
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Node/Parameter/2Float")]
public class Parameter2Float : ParameterAbstract
{
    public string name;
    public TMP_InputField a;
    public TMP_InputField b;
    
    public override bool GetValue(out string value)
    {
        value = $"{name} ";
        if (!float.TryParse(a.text, out var result1) || !float.TryParse(b.text, out var result2)) return false;
        value = $"{result1} {result2}";
        return true;
    }
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Node/Parameter/1Int")]
public class Parameter1Int : ParameterAbstract
{
    public string name;
    public TMP_InputField a;
    
    public override bool GetValue(out string value)
    {
        value = $"{name} ";
        if (!int.TryParse(a.text, out var result)) return false;
        value = $"{result}";
        return true;
    }
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Node/Parameter/2Int")]
public class Parameter2Int : ParameterAbstract
{
    public string name;
    public TMP_InputField a;
    public TMP_InputField b;
    
    public override bool GetValue(out string value)
    {
        value = $"{name} ";
        if (!int.TryParse(a.text, out var result1) || !int.TryParse(b.text, out var result2)) return false;
        value = $"{result1} {result2}";
        return true;
    }
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Node/Parameter/Bool")]
public class ParameterBool : ParameterAbstract
{
    public string name;
    public Toggle toggle;
    
    public override bool GetValue(out string value)
    {
        value = "";
        if (!toggle.isOn) return false;
        value = $"{name}";
        return true;
    }
}