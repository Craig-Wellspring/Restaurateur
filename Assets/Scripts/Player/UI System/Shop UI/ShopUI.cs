using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    // States
    public bool isOpen { get; private set; } = false;

    // Cache
    private PlayerManager player;
    private CanvasGroup controller;
    public ShopContentWindow contentWindow { get; private set; }
    public ShopCartController cart { get; private set; }

    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
        controller = GetComponent<CanvasGroup>();
        contentWindow = transform.GetComponentInChildren<ShopContentWindow>();
        cart = transform.GetComponentInChildren<ShopCartController>();
    }

    private void Start() {
        HideShopWindow();
    }

    public void ShowShopWindow(List<ShopInventoryObject> _inventory) {
        controller.alpha = 1;
        controller.interactable = true;
        controller.blocksRaycasts = true;
        contentWindow.LoadInventory(_inventory);
        isOpen = true;

        player.ui.money.Show();
    }

    public void HideShopWindow() {
        controller.alpha = 0;
        controller.interactable = false;
        controller.blocksRaycasts = false;
        contentWindow.UnloadInventory();
        isOpen = false;

        player.ui.money.Hide();
    }
}
