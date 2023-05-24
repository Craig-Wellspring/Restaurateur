using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ShopCartController : MonoBehaviour
{
    // Settings
    public GameObject packingBoxPrefab;
    public GameObject holdingBoxPrefab;
    public GameObject foodCratePrefab;

    public GameObject cartItemPrefab;
    public Transform cartItemContainer;
    public TextMeshProUGUI cartTotalCounter;

    // Cache
    private PlayerManager player;

    // States
    private List<CartItem> cartItems = new();
    private float cartTotal = 0f;

    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
    }

    private void Start() {
        ClearCart();
    }

    public void UpdateTotal() {
        float _total = 0f;
        cartItems.ForEach((_obj) => _total += _obj.totalPrice);
        cartTotal = _total;
        cartTotalCounter.text = string.Format("{0:C}", _total);
    }

    public void AddToCart(ShopInventoryObject _obj) {
        CartItem _found = cartItems.FirstOrDefault((o) => o.obj == _obj);
        if (!_found) {
            GameObject _newCartItem = Instantiate(cartItemPrefab, Vector3.zero, Quaternion.identity, cartItemContainer);
            _newCartItem.TryGetComponent<CartItem>(out CartItem _item);
            _item.Set(_obj);
            _item.UpdatePrice();
            cartItems.Add(_item);
            UpdateTotal();
        }
    }

    public void RemoveFromCart(CartItem _obj) {
        cartItems.Remove(_obj);
        UpdateTotal();
    }

    public void ClearCart() {
        foreach(Transform _child in cartItemContainer) {
            Destroy(_child.gameObject);
        }

        cartItems = new();
        UpdateTotal();
    }

    public void BuyCart() {
        if (player.money.TakeMoney(cartTotal)) {
            int packageCount = 0;
            List<CartItem> _foods = new();
            List<CartItem> _holdables = new();
            List<CartItem> _gridbound = new();

            // Sort items
            foreach(CartItem _item in cartItems) {
                Debug.Log("Buy " + _item.count + " " + _item.obj.prefab.name);
                if (_item.obj.prefab.TryGetComponent<Item>(out Item _invItem)) {
                    switch (_invItem.itemType) {
                        case ItemType.Food:
                            // Add to ListOfFoods
                            _foods.Add(_item);
                            _item.obj.quantity -= _item.count;
                        break;

                        case ItemType.Cookware:
                        case ItemType.Tableware:
                        case ItemType.Gear:
                            // Add to ListOfHoldables
                            _holdables.Add(_item);
                            _item.obj.quantity -= _item.count;
                        break;

                        case ItemType.Equipment:
                            // Add to ListOfGridbound
                            _gridbound.Add(_item);
                            _item.obj.quantity -= _item.count;
                        break;
                    }
                }
            }
            
            // Spawn items
            // Foods
            if (_foods.Count > 0) {
                GameObject _foodCrate = Instantiate(foodCratePrefab, GameManager.Master.deliveryLocation.position + new Vector3(packageCount, 1, 0), Quaternion.identity);
                _foodCrate.TryGetComponent<HoldablesContainer>(out HoldablesContainer _foodContainer);

                foreach(CartItem _food in _foods) {
                    for(int c = 0; c < _food.count; c++) {
                        _foodContainer.StoreItem(_food.obj.prefab, _food.obj.modifier);
                    }
                }

                foreach(FoodObject _food in _foodContainer.storedItems) {
                    _food.temperature = 32f;
                }
                packageCount++;
            }

            // Holdables
            if (_holdables.Count > 0) {
                GameObject _holdablesBox = Instantiate(holdingBoxPrefab, GameManager.Master.deliveryLocation.position + new Vector3(packageCount, 1, 0), Quaternion.identity);
                _holdablesBox.TryGetComponent<HoldablesContainer>(out HoldablesContainer _holdablesContainer);
                
                foreach(CartItem _holdable in _holdables) {
                    for(int c = 0; c < _holdable.count; c++) {
                        _holdablesContainer.StoreItem(_holdable.obj.prefab);
                    }
                }

                packageCount++;
            }

            // Gridbound
            if (_gridbound.Count > 0) {
                foreach(CartItem _gb in _gridbound) {
                    GameObject _packingBox = Instantiate(packingBoxPrefab, GameManager.Master.deliveryLocation.position + new Vector3(packageCount, 1, 0), Quaternion.identity);
                    _packingBox.TryGetComponent<PackingBox>(out PackingBox _box);

                    _box.Pack(_gb.obj.prefab.transform);

                    packageCount++;
                }
            }

            // Update shop UI
            ClearCart();
            player.ui.shop.contentWindow.Refresh();

        } else { // Not enough money
            Debug.Log("Not enough money to purchase entire cart.");
        }
    }
}
