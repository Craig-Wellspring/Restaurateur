using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldablesContainer : MonoBehaviour, IClickable
{
    // Settings
    public int capacity;

    [Tooltip("May only contain items of this type. If None, may contain any item type.")]
    public ItemType exclusiveType;

    [Tooltip("Rate at which temperatures adjust, 1 is no insulation (air), 0 is perfect insulation (vacuum)"), Range(0,1)]
    public float tempLossRate = 1f;

    // States
    public List<object> storedItems = new List<object>();
    private float ambientTemp;

    // Cache
    public Transform spawnPoint;
    private Animator anim;
    private bool isCarryable;

    private void Awake() {
        anim = GetComponent<Animator>();
        isCarryable = transform.root.TryGetComponent<Carryable>(out Carryable _carryable);
        
        TimeManager.OnThreeSeconds += AdjustTemperatures;
    }

    private void OnDestroy() {
        TimeManager.OnThreeSeconds -= AdjustTemperatures;
    }

    private void Start() {
        if (ambientTemp == 0) ambientTemp = GameManager.Master.airTemp;
    }
    
    private void AdjustTemperatures(object _sender, System.EventArgs _args) {
        foreach(object _obj in storedItems) {
            if (_obj.GetType() == typeof(FoodObject)) {
                FoodObject _food = (FoodObject)_obj;
                _food.temperature += ((CookingSystem.CalculateHeatChange(_food.mass, _food.conductivity, _food.temperature, ambientTemp) * 15) * tempLossRate);

                if (_food.contamination != null)
                    _food.contamination = Mathf.Clamp((_food.contamination ?? 0) + Perishable.CalculateContamination(_food.temperature), 0, Perishable.maxContam);
            }
        }
    }
    
    public void Click(PlayerManager _player, int _L0orR1) {
        if (!_player.hands.isCarrying) {
            // Check hands
            HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
            Transform handObj = _hand.GetHeldObject();
            // Hand empty - pick up item
            if (handObj == null) {
                if (!isCarryable || (isCarryable && !_player.interact.CanInteract(this.transform))) { // This container isn't a carryable box
                    TakeItem(_hand);
                    anim?.SetTrigger("QuickOpen");
                }
            } else { // Hand full - store item
                if (exclusiveType == ItemType.None || handObj.GetComponent<Item>().itemType == exclusiveType) {
                    if (StoreItem(handObj.gameObject)) {
                        _hand.TakeHeldObject(spawnPoint);
                        anim?.SetTrigger("QuickOpen");
                    }
                }
            }
        }
    }

    public void OpenAnim(PlayerManager _player = null, int _L0orR1 = 0) {
        anim?.SetBool("isOpen", true);
    }

    public void CloseAnim(PlayerManager _player = null, int _L0orR1 = 0) {
        anim?.SetBool("isOpen", false);
    }

    public void SetAmbientTemp(float _newTemp) {
        ambientTemp = _newTemp;
    }

    public bool TakeItem(HandHold _receivingHand, int _index = 0, bool _fifo = true) {
        if (storedItems.Count > 0) {
            HoldableObject _obj = (HoldableObject)storedItems[_fifo ? _index : storedItems.Count-1];
            Transform _collectedItem = _obj.GenerateItemFromObj(spawnPoint.position, Quaternion.identity);
            _receivingHand.HoldObject(_collectedItem);

            storedItems.Remove(storedItems[_index]);
            return true;
        }
        return false;
    }

    public bool StoreItem(GameObject _item, Vector2? _modifier = null) {
        if (_item.TryGetComponent<HoldableItem>(out HoldableItem _holdable)) {
          // Check storage levels and update
          if (storedItems.Count < capacity) {
                storedItems.Add(_holdable.GenerateObject(_modifier));
                return true;
          } else {
              Debug.Log("Storage is Full");
              return false;
          }
        }
        else {
            Debug.Log("Can only store holdable items");
            return false;
        }
    }
}
