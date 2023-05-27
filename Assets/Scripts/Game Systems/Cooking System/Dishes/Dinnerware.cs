using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinnerware : DishItem
{
    // Settings
    public DinnerwareType dinnerwareType;

    // States
    public DishManager dishManager { get; private set; }
  
    private void Awake() {
        base.LoadRefs();

        dishManager = transform.GetComponentInChildren<DishManager>();
    }
    
    public override void Click(PlayerManager _player, int _L0orR1) {
        // Check hand
        HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
        Transform handObj = _hand.GetHeldObject();
        
        if (handObj != null && handObj != this.transform) {
            if (handObj.TryGetComponent<FoodItem>(out FoodItem _heldFood)) {
                Transform foodPlatePos = dishManager.TryAddToPlate(_heldFood);
                if (foodPlatePos != null) {
                    _hand.PlaceHeldObject(foodPlatePos.position, foodPlatePos.rotation, () => {
                        Destroy(handObj.gameObject);
                    });
                }
            } 
        } else {
            base.Click(_player, _L0orR1);
        }
    }
}

public enum DinnerwareType {
    SmallPlate,
    LargePlate,
    SmallBowl,
    LargeBowl,
}
