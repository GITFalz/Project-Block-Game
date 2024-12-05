using System.Collections.Generic;
using UnityEngine;

public abstract class CustomUIGroup : CustomUI
{
    [Header("Lists")] 
    public List<GameObject> contentObjects = new();
    public List<CustomUI> customUIScripts = new();

    [Header("Group Properties")] 
    public Transform content;
    
    protected CustomUICollectionManager CollectionManager;
    
    public override void Init(CustomUICollectionManager collectionManager)
    {
        CollectionManager = collectionManager;
        
        if (content == null)
            return;
        
        InitGameObjects();
        InitScripts();
    }

    public void InitGameObjects()
    {
        contentObjects.Clear();
        
        foreach (Transform c in content)
        {
            contentObjects.Add(c.gameObject);
        }
    }
    
    public void InitScripts()
    {
        customUIScripts.Clear();
        
        foreach (GameObject c in contentObjects)
        {
            CustomUI cI = c.GetComponent<CustomUI>();

            if (cI == null || cI.Equals(null))
                continue;
            
            cI.Init(CollectionManager);
            customUIScripts.Add(cI);
        }
    }
    
    public override float Align(Vector3 pos)
    {
        float totalHeight = 0;
        
        transform.position = pos;
        Vector3 position = pos - new Vector3(0, height, 0);
        
        if (CollectionManager.DoHorizontalSpacing())
            position.x += 20;
        
        foreach (var parameter in customUIScripts)
        {
            if (parameter.gameObject.activeSelf == false)
                continue;
            
            float newHeight = parameter.Align(position);
            position.y -= newHeight;
            totalHeight += newHeight;
        }

        return totalHeight;
    }

    public override string ToCWorld()
    {
        string text = "";
        
        foreach (var c in customUIScripts)
        {
            text += c.ToCWorld();
        }

        return text;
    }
}