using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;

public class HandHold : MonoBehaviour
{
    [SerializeField] private Transform currentlyHeldObj = null;

    private PlayerManager player;

    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
    }

    public Transform GetHeldObject() {
        return currentlyHeldObj;
    }

    public void HoldObject(Transform _obj) {
        // Clear other hand if grabbing from other hand
        if (_obj.GetComponentInParent<HandHold>())
            _obj.GetComponentInParent<HandHold>().ReleaseHeldObject();

        
        BillboardController bar = _obj.GetComponentInChildren<BillboardController>();
        bar?.Hide();
        bar?.Lock();
            
        Rigidbody objRB = _obj.gameObject.GetComponent<Rigidbody>();
        objRB.isKinematic = true;
        _obj.gameObject.layer = LayerMask.NameToLayer("NoClip"); // Remove collision on item
        
        _obj.SetParent(transform);
        currentlyHeldObj = _obj;
        _obj.TryGetComponent<HoldableItem>(out HoldableItem _h);
        void PickedUp() {
            if (_obj.parent != null) {
                _obj.SetLocalPositionAndRotation(_h.handOffset, Quaternion.Euler(_h.handRotation));
                _obj.gameObject.layer = 15; // Put item on Player collision layer to prevent interference
            }
        }
        StartCoroutine(UtilsClass.LerpPosition(_obj, transform, player.hands.pickupSpeed, PickedUp));
    }

    public void ReleaseHeldObject() {
        if (currentlyHeldObj != null) {
            BillboardController bar = currentlyHeldObj.GetComponentInChildren<BillboardController>();
            bar?.Unlock();

            currentlyHeldObj.SetParent(null);
            Rigidbody objRB = currentlyHeldObj.gameObject.GetComponent<Rigidbody>();
            objRB.isKinematic = false;
            objRB.gameObject.layer = 9; // Put item back on Holdable collision layer
            currentlyHeldObj = null;
        } else {
            currentlyHeldObj = null;
        }
    }

    public void TakeHeldObject(Transform _taker) {
        if (currentlyHeldObj != null) {
            currentlyHeldObj.SetParent(_taker);
            currentlyHeldObj.gameObject.layer = LayerMask.NameToLayer("NoClip"); // Remove collision on item
            GameObject obj = currentlyHeldObj.gameObject;
            StartCoroutine(UtilsClass.LerpPosition(currentlyHeldObj, _taker, player.hands.pickupSpeed, () => { Destroy(obj); }));
            currentlyHeldObj = null;
        } else {
            currentlyHeldObj = null;
        }
    }

    public void PlaceHeldObject(Vector3 _destination, Quaternion _rotation) {
        Transform obj = GetHeldObject();
        ReleaseHeldObject();
        StartCoroutine(UtilsClass.LerpPosition(obj, _destination, _rotation, player.hands.pickupSpeed));
    }

    public void DestroyHeldObject() {
        if (currentlyHeldObj != null) {
            Destroy(currentlyHeldObj.gameObject);
            currentlyHeldObj = null;
        }
    }

    public void UseHeldObject() {
        player.crafting.UseObject(currentlyHeldObj);
    }
}
