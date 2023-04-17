using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    // States
    public ShopInventoryObject shopObj { get; private set; }

    // Cache
    private UIManager UI;
    private TextMeshProUGUI quantityRemaining;
    private TextMeshProUGUI objName;
    private Image icon;
    private TextMeshProUGUI price;

    private void Awake() {
        UI = transform.root.GetComponentInChildren<UIManager>();

        quantityRemaining = transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
        quantityRemaining.text = "";

        objName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        objName.text = "";

        icon = transform.Find("Image").GetComponent<Image>();
        icon.sprite = null;

        price = transform.Find("Price").GetComponent<TextMeshProUGUI>();
        price.text = "";
    }
    
    public void Set(ShopInventoryObject _shopObj) {
        if (_shopObj.prefab.TryGetComponent<Item>(out Item _invItem)) {
            shopObj = _shopObj;
            if (_shopObj.quantity >= 0)
                quantityRemaining.text = _shopObj.quantity.ToString();
            objName.text = _shopObj.prefab.name;
            icon.sprite = _invItem.sprite;
            price.text = string.Format("{0:C}", _shopObj.price);
        }
    }

    public void ButtonClick() {
        if (shopObj.quantity == 0) {
            return;
        }
        
        UI.shop.cart.AddToCart(shopObj);
    }
}
