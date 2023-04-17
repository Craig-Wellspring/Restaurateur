using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGridManager : MonoBehaviour
{
    private CanvasGroup controller;
    private PlayerManager player;

    private List<InventorySlot> inventorySlots;
    public int? mouseoverInventorySlotIndex { get; private set; }

    private void Awake() {
        controller = GetComponent<CanvasGroup>();
        player = transform.root.GetComponent<PlayerManager>();

        inventorySlots = new List<InventorySlot>();
        foreach (Transform _child in this.transform) {
            inventorySlots.Add(_child.GetComponent<InventorySlot>());
        }
    }

    private void Start() {
        HideInventoryGrid();
    }

    public void ShowInventoryGrid() {
        controller.alpha = 1;
        controller.interactable = true;
        controller.blocksRaycasts = true;
    }

    public void HideInventoryGrid() {
        controller.alpha = 0;
        controller.interactable = false;
        controller.blocksRaycasts = false;
        SetMouseoverInventorySlot(null);
        ClearInventoryGrid();
    }

    public void PopulateInventoryGrid(HoldablesContainer _storage) {
        for (int i = 0; i < _storage.storedItems.Count; i++) {
            inventorySlots[i].UpdateObject(_storage.storedItems[i]);
            inventorySlots[i].gameObject.SetActive(true);
        }
    }

    private void ClearInventoryGrid() {
        foreach (InventorySlot _slot in inventorySlots) {
            _slot.gameObject.SetActive(false);
        }
    }

    public void SetMouseoverInventorySlot(int? _index) {
        mouseoverInventorySlotIndex = _index;
    }

    public void RemoveObject() {
    }

    public void OpenInventory(HoldablesContainer _storage) {
        ShowInventoryGrid();
        _storage.OpenAnim();
    }

    public void CloseInventory(HoldablesContainer _storage) {
        HideInventoryGrid();
        _storage.CloseAnim();
        player.mouse.SwitchToArrow();
    }
}
