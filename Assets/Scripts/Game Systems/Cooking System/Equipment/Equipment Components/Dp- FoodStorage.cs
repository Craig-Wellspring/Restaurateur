using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStorage : MonoBehaviour, IClickable
{
    // States
    public List<FoodObject> storedItems = new List<FoodObject>();
    public int capacity;

    // Cache
    public Transform spawnPoint;
    private Packable rootPackable;
    private Animator anim;
    private bool isCarryable;

    private void Awake() {
        rootPackable = transform.root.GetComponent<Packable>();
        rootPackable?.AddLock(this);
        anim = GetComponent<Animator>();
        isCarryable = transform.root.TryGetComponent<Carryable>(out Carryable _carryable);
    }
    
    public void Click(PlayerManager _player, int _L0orR1) {
        if (!_player.hands.isCarrying && !isCarryable) {
            // Check hands
            HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
            Transform handObj = _hand.GetHeldObject();
            // Hand empty - pick up item
            if (handObj == null) {
                TakeItem(_hand);
                anim?.SetTrigger("QuickOpen");
            } else { // Hand full - store item
                if (StoreItem(handObj)) {
                    _hand.TakeHeldObject(spawnPoint);
                    anim?.SetTrigger("QuickOpen");
                }
            }
        }
    }

    public void OpenDoor(PlayerManager _player = null, int _L0orR1 = 0) {
        anim?.SetBool("isOpen", true);
    }

    public void CloseDoor(PlayerManager _player = null, int _L0orR1 = 0) {
        anim?.SetBool("isOpen", false);
    }

    public bool TakeItem(HandHold _receivingHand, int _index = 0, bool _fifo = true) {
        if (storedItems.Count > 0) {
            // Transform collectedItem = FoodItem.GenerateInstance(storedItems[_fifo ? _index : storedItems.Count-1], spawnPoint.position, Quaternion.identity);
            // _receivingHand.HoldObject(collectedItem);

            storedItems.Remove(storedItems[_index]);
            if (storedItems.Count == 0) {
                rootPackable?.SetIsLocked(this, false);
            }
            return true;
        }
        return false;
    }

    public bool StoreItem(Transform _obj) {
        if (_obj.TryGetComponent<FoodItem>(out FoodItem _food)) {
          // Check storage levels and update
          if (storedItems.Count < capacity) {
                if (storedItems.Count == 0) {
                    rootPackable?.SetIsLocked(this, true);
                }
                storedItems.Add((FoodObject)_food.GenerateObject());
                return true;
          } else {
              Debug.Log("Storage is Full");
              return false;
          }
        }
        else {
            Debug.Log("Can only store food items");
            return false;
        }
    }
}
