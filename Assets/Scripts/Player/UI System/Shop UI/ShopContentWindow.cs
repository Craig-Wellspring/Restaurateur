using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopContentWindow : MonoBehaviour
{
    public GameObject shopItemPrefab;
    private ShopTabsContainer tabsContainer;

    public Dictionary<ItemType, List<ShopInventoryObject>> lists = new();

    private void Awake() {
        tabsContainer = transform.root.GetComponentInChildren<ShopTabsContainer>();
    }

    public void LoadInventory(List<ShopInventoryObject> _inventory) {
        foreach (ShopInventoryObject _item in _inventory) {
            if (_item.prefab != null && _item.prefab.TryGetComponent<Item>(out Item _invItem)) {
                if (!lists.ContainsKey(_invItem.itemType)) {
                    lists.Add(_invItem.itemType, new List<ShopInventoryObject>());
                    tabsContainer.ActivateTab(_invItem.itemType.ToString());
                }

                lists[_invItem.itemType].Add(_item);
            }
        }

        // By default, populate first tab
        if (lists.Count > 0)
            ChangeTab(tabsContainer.tabs.ElementAt(0).Key);
    }

    public void UnloadInventory() {
        lists = new();
        tabsContainer.DeactivateAllTabs();
        ClearObjectList();
    }

    public void ChangeTab(string _tabName) {
        ClearObjectList();
        foreach (var _key in lists.Keys) {
            if (_key.ToString() == _tabName) {
                Populate(lists[_key]);
            }
        }
    }

    public void Populate(List<ShopInventoryObject> _objList) {
        foreach(ShopInventoryObject _obj in _objList) {
            GameObject newShopItem = Instantiate(shopItemPrefab, Vector3.zero, Quaternion.identity, this.transform);
            newShopItem.GetComponent<ShopItem>().Set(_obj);
        }
    }

    public void Refresh() {
        foreach(Transform _itemTransform in this.transform) {
            _itemTransform.TryGetComponent<ShopItem>(out ShopItem _listItem);
            _listItem.Set(_listItem.shopObj);
        }
    }

    public void ClearObjectList() {
        if (transform.childCount > 0)
            foreach (Transform _child in this.transform) {
                GameObject.Destroy(_child.gameObject);
            }
    }
}
