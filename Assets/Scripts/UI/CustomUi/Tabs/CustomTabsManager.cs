using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomTabsManager : MonoBehaviour
{
    [Header("Properties")]
    public GameObject buttonPrefab;
    
    public List<CustomTabManager> tabs = new List<CustomTabManager>();
    public List<TabSelectorManager> buttons = new List<TabSelectorManager>();
    
    private Transform _content;
    private Transform _buttons;

    private void Awake()
    {
        _content = transform.Find("Content");
        _buttons = transform.Find("Buttons");
        
        foreach(Transform child in _content)
        {
            CustomTabManager tab = child.GetComponent<CustomTabManager>();
            if (tab != null)
                tabs.Add(tab);
        }

        for (int i = 0; i < tabs.Count; i++)
        {
            TabSelectorManager t = Instantiate(buttonPrefab, _buttons).GetComponent<TabSelectorManager>();
            if (t != null)
            {
                buttons.Add(t);
                t.tabName = tabs[i].tabName;
                t.page = tabs[i].gameObject;
                t.index = i;
                t.Init();
            }
        }
    }
    
    public void SelectTab(int index)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].page.SetActive(i == index);
            buttons[i].isSelected = i == index;
        }
    }
}