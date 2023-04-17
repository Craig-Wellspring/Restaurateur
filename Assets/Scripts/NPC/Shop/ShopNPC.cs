using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour, IClickable
{
    public List<ShopInventoryObject> inventory;
    
    public void Click(PlayerManager _clicker, int _L0orR1) {
        if (!_clicker.ui.shop.isOpen)
            _clicker.ui.shop.ShowShopWindow(inventory);
    }
}

// [System.Serializable]
// public class ShopInventory {
//     public List<ShopInventoryObject> inventoryList;

//     public ShopInventory(List<ShopInventoryObject> _inventoryList) {
//         inventoryList = _inventoryList;
//     }
// }
