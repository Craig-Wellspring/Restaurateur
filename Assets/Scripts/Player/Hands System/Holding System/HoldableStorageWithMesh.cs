using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableStorageWithMesh : HoldableStorage
{
    [SerializeField] private Transform storedFoodVisual;

    private void OnEnable() {
        if (storedItem != null) {
            UpdateStoredFoodMesh(storedItem);
        }
    }

    private void UpdateStoredFoodMesh(Transform _newObj) {
        if (_newObj != null) {
            storedFoodVisual.GetComponent<MeshFilter>().sharedMesh = _newObj.GetComponent<MeshFilter>().sharedMesh;
            storedFoodVisual.GetComponent<MeshRenderer>().sharedMaterials = _newObj.GetComponent<MeshRenderer>().sharedMaterials;
        } else {
            storedFoodVisual.GetComponent<MeshFilter>().sharedMesh = null;

            MeshRenderer renderer = storedFoodVisual.GetComponent<MeshRenderer>();
            for (int i = 0; i > renderer.sharedMaterials.Length; i++) {
                renderer.sharedMaterials[i] = null;
            }      
        }
    }

    public override bool PickupItem(HandHold _receivingHand) {
        if (currentStorage == 1)
            UpdateStoredFoodMesh(null);
        return RemoveFromStorage(_receivingHand);
    }

    public override bool PlaceItem(Transform _newFood) {
        // FoodItem obj = _newFood.GetComponent<FoodItem>();
        // if (obj != null)
        // {
            if (AddToStorage(_newFood)) {
                UpdateStoredFoodMesh(_newFood);
                return true;
            }
        // } else {
        //     Debug.Log("Cooler can only store food items");
        // }
        return false;
    }
}
