using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTabsContainer : MonoBehaviour
{
    public Dictionary<string, Transform> tabs = new();

    private void Awake() {
        foreach (Transform _child in transform) {
            string tabName = _child.name.Split(" ")[0];
            tabs.Add(tabName, _child);
            DeactivateTab(tabName);
        }
    }

    public void ActivateTab(string _tabName) {
        tabs[_tabName].gameObject.SetActive(true);
    }

    public void DeactivateTab(string _tabName) {
        tabs[_tabName].gameObject.SetActive(false);
    }

    public void DeactivateAllTabs() {
        foreach(var _tab in tabs) {
            _tab.Value.gameObject.SetActive(false);
        }
    }
}
