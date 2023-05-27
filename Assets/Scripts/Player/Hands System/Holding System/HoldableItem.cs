using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableItem : Item, IClickable
{
    public Vector3 handOffset;
    public Vector3 handRotation;

    private void Awake() {
        base.LoadRefs();
    }

    public virtual void Click(PlayerManager _player, int _L0orR1) {
        if (!_player.hands.isCarrying) {
            // Check hand
            HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
            Transform handObj = _hand.GetHeldObject();

            // Hand empty - pick up item
            if (handObj == null) {
                _hand.HoldObject(this.transform);
                return;
            }

            // Hand full, clicked item is in hand - drop item
            if (handObj == this.transform)
                _hand.ReleaseHeldObject();
                
            return;
        }
    }

    public virtual HoldableObject GenerateObject() {
        GameObject _go = prefab == null ? this.gameObject : prefab;
        
        return HoldableObject.Create(_go, sprite);
    }

    public virtual HoldableObject GenerateObject(Vector2? _modifier) {
        GameObject _go = prefab == null ? this.gameObject : prefab;
        
        return HoldableObject.Create(_go, sprite);
    }
}


// Scriptable Object
public class HoldableObject : ScriptableObject
{
    public GameObject prefab;
    public Sprite image;

    public static HoldableObject Create(GameObject _prefab, Sprite _image) {
        HoldableObject _newObj = CreateInstance<HoldableObject>();

        _newObj.prefab = _prefab;
        _newObj.image = _image;

        return _newObj;
    }

    public virtual Transform GenerateItemFromObj(Vector3 _pos, Quaternion _rot) {
        Transform _newObj = Instantiate(this.prefab.transform, _pos, _rot);
        _newObj.name = this.prefab.name;

        return _newObj;
    }
} 