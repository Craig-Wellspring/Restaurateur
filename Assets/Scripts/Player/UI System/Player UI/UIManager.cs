using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Cache
    private PlayerManager player;
    public InventoryGridManager inventory { get; private set; }
    public MoneyCounter money { get; private set; }
    public ShopUI shop { get; private set; }

    // States
    private List<Transform> openStack = new();


    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
        inventory = GetComponentInChildren<InventoryGridManager>();
        money = GetComponentInChildren<MoneyCounter>();
        shop = GetComponentInChildren<ShopUI>();
    }

    private void Start() {
        // money.gameObject.SetActive(player.building.blueprintModeOn);
    }

    public bool IsUIOpen() {
        return openStack.Count > 0;
    }

    public void AddToStack(Transform _transform) {
        openStack.Add(_transform);
    }

    public void RemoveFromStack(Transform _transform) {
        openStack.Remove(_transform);
    }
}
