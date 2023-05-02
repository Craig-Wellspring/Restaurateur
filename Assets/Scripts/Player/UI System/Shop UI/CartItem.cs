using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CartItem : MonoBehaviour
{
    // States
    public int count { get; private set; }
    public ShopInventoryObject obj { get; private set; }
    public float totalPrice { get; private set; }

    // Cache
    private TMP_InputField inputField;
    private Image image;
    private TMP_Text cartPrice;

    private void Awake() {
        inputField = GetComponentInChildren<TMP_InputField>();
        image = transform.Find("Image").GetComponent<Image>();
        cartPrice = transform.Find("Price").GetComponent<TMP_Text>();
        cartPrice.text = "$";
    }

    public void UpdatePrice() {
        totalPrice = count * obj.price;
        cartPrice.text = string.Format("{0:C}", totalPrice);
        transform.root.GetComponentInChildren<UIManager>().shop.cart.UpdateTotal();
    }

    public void Set(ShopInventoryObject _shopObj) {
        _shopObj.prefab.TryGetComponent<Item>(out Item _invItem);
        obj = _shopObj;
        image.sprite = _invItem.sprite;
        UpdatePrice();
        count = 1;
    }

    public void IncreaseCount() {
        if (count < obj.quantity) {
            count++;
            UpdatePrice();
            inputField.text = count.ToString();
        }
    }

    public void DecreaseCount() {
        if (count > 1) {
            // Reduce count
            count--;
            UpdatePrice();
            inputField.text = count.ToString();
        } else {
            // Remove from cart
            transform.root.GetComponentInChildren<UIManager>().shop.cart.RemoveFromCart(this);
            Destroy(this.gameObject);
        }
    }
}
