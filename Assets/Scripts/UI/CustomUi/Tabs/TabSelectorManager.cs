using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabSelectorManager : MonoBehaviour
{
    [Header("Tab Settings")] 
    public string tabName;
    public GameObject page;
    public int index;
    
    private Button _button;
    private TMP_Text _buttonText;
    private CustomTabsManager _tabsManager;
    
    [HideInInspector]
    public bool isSelected;
    
    public void Init()
    {
        _button = GetComponent<Button>();
        _buttonText = transform.Find("Text").GetComponent<TMP_Text>();
        
        _button.onClick.AddListener(OnTabClick);
        _buttonText.text = tabName;
        
        _tabsManager = transform.parent.parent.GetComponent<CustomTabsManager>();
    }
    
    private void OnTabClick()
    {
        if (_tabsManager == null)
            return;
        
        isSelected = !isSelected;
        
        if (isSelected)
            _tabsManager.SelectTab(index);
        else
            page.SetActive(false);
    }
}