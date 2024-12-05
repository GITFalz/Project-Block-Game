using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomTabManager : MonoBehaviour
{
    [Header("Tab Settings")] 
    public string tabName;
    
    private CustomUICollectionManager _collectionManager;

    private void Awake()
    {
        _collectionManager = GetComponentInChildren<CustomUICollectionManager>();
        
        if (_collectionManager == null)
            return;
        
        _collectionManager.Init();
    }

    private void Update()
    {
        float scroll = MouseData.GetMouseScroll();

        if (scroll != 0)
        {
            _collectionManager.scrollHeight += scroll;
            _collectionManager.AlignCollections();
        }
    }
}