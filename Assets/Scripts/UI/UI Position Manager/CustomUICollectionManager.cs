using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomUICollectionManager: CustomUIGroup
{
    [Header("Collection Parameters")] 
    public TypeOrText collectionName;
    public GameObject collectionPrefab;
    public bool doHorizontalSpacing;

    [Header("Panel")]
    public TMP_Text text;
    public Button button;
    public Button show;

    public float scrollHeight = 0;
    public string currentNewCollectionName;

    public void Init()
    {
        if (content == null)
        {
            Console.Log("Collection Content is null");
            return;
        }
        
        if (text == null)
        {
            Console.Log("Collection Text is null");
            return;
        }
        
        if (button == null)
        {
            Console.Log("Collection Button is null");
            return;
        }
        
        if (show == null)
        {
            Console.Log("Collection Show is null");
            return;
        }
        
        button.onClick.AddListener(AddCollection);
        show.onClick.AddListener(() => Console.Log(Show()));
        
        base.Init(this);
        
        AlignCollections();
    }
    
    public void AddCollection()
    {
        GameObject collection = Instantiate(collectionPrefab, content);
        CustomUI cI = collection.GetComponent<CustomUI>();
        
        if (cI == null || cI.Equals(null))
        {
            Destroy(collection);
            return;
        }
        
        cI.Init(this);
        contentObjects.Add(collection);
        AlignCollections();
    }

    public string Show()
    {
        return base.ToCWorld();
    }
    
    public void AlignCollections()
    {
        Vector3 position = transform.position - new Vector3(0, height - scrollHeight, 0);
        content.position = position;
        Align(position);
    }
    
    public bool DoHorizontalSpacing()
    {
        return doHorizontalSpacing;
    } 
}

[Serializable]
public class TypeOrText
{
    public string type;
    public string text;
}