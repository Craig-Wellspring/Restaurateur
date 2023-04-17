using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// DEPRECATE
public class HoldableStorage : MonoBehaviour, IClickable
{
    // States
    public Transform storedItem;
    public int maxStorage;
    public int currentStorage;

    // Cache
    public Transform spawnPoint;
    private TextMeshPro counter;
    private Packable pack;


    private void Awake() {
        counter = GetComponentInChildren<TextMeshPro>();
        pack = GetComponent<Packable>();
        pack?.AddLock(this);
    }

    private void Start() {
        UpdateCounterText();
    }
    
    public void Click(PlayerManager _player, int _L0orR1) {
        if (!_player.hands.isCarrying) {
            // Check hands
            HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
            Transform handObj = _hand.GetHeldObject();
            // Hand empty - pick up item
            if (handObj == null) {
                if (storedItem != null)
                    PickupItem(_hand);
            } else { // Hand full - store item
                if (PlaceItem(handObj))
                    _hand.TakeHeldObject(spawnPoint);
            }
        }
    }    

    public virtual bool PickupItem(HandHold _receivingHand) {
        return RemoveFromStorage(_receivingHand);
    }

    public bool RemoveFromStorage(HandHold _receivingHand) {
        if (currentStorage > 0) {
            Transform collectedItem = Instantiate(storedItem.transform, spawnPoint.position, Quaternion.identity);
            collectedItem.name = storedItem.name;
            _receivingHand.HoldObject(collectedItem);

            currentStorage--;
            UpdateCounterText();
            if (currentStorage == 0) {
                storedItem = null;
                pack?.SetIsLocked(this, false);
            }
            
            return true;
        }
        return false;
    }

    public virtual bool PlaceItem(Transform _obj) {
        if (_obj.GetComponent<HoldableObject>() != null)
            return AddToStorage(_obj);
        else
            return false;
    }

    public bool AddToStorage(Transform _obj) {
        // Store only one type of item
        if (storedItem != null)
            if (_obj.name != storedItem.name) {
                Debug.Log("Something else is stored");
                return false;
            }

        // Check storage levels and update
        if (currentStorage < maxStorage) {
            if (currentStorage == 0) {
                storedItem = _obj;
                pack?.SetIsLocked(this, true);
            }
            currentStorage++;
            UpdateCounterText();
            return true;
        } else {
            Debug.Log("Storage is Full");
            return false;
        }
    }

    private void UpdateCounterText() {
        if (currentStorage == 0) {
            counter.text = null;
            return;
        } else {
            counter.text = currentStorage.ToString();
        }
    }
}
