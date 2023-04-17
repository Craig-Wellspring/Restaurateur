using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;

public class CarryPoint : MonoBehaviour
{
    private PlayerManager player;
    private Transform currentlyCarriedObj = null;
    public Transform carryTarget { get; private set; } = null;

    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
    }

    public Transform GetCarriedObject() {
        return currentlyCarriedObj;
    }

    public void LiftObject(Transform _obj) {
        carryTarget = _obj;
        // Look at object
        Vector3 lookDir = _obj.position - player.transform.position;
        lookDir.y = 0f;
        player.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        // If object isn't on the ground, don't play pickup animation
        if (_obj.position.y > 0.5f) {
            CarryObject(_obj);
            return;
        }
        // Play pickup animation to lift object off the ground
        player.anim.TriggerLiftAnim();
    }

    public void CarryObject(Transform _obj) {
        Rigidbody objRB = _obj.gameObject.GetComponent<Rigidbody>();
        objRB.isKinematic = true;
        objRB.gameObject.layer = 15; // Put item on Player collision layer to prevent interference

        _obj.SetParent(transform);
        void PickedUp() { _obj.localRotation = Quaternion.identity; }
        StartCoroutine(UtilsClass.LerpPosition(_obj, transform, player.hands.pickupSpeed, PickedUp));
        currentlyCarriedObj = _obj;
        player.hands.SetIsCarrying(true);
    }

    public void DropCarriedObject() {
        if (currentlyCarriedObj != null) {
            currentlyCarriedObj.SetParent(null);
            Rigidbody objRB = currentlyCarriedObj.gameObject.GetComponent<Rigidbody>();
            objRB.isKinematic = false;
            objRB.gameObject.layer = 10; // Put item back on Carryable collision layer
            currentlyCarriedObj = null;
            player.hands.SetIsCarrying(false);
            carryTarget = null;
        } else {
            currentlyCarriedObj = null;
        }
    }

    public void PlaceCarriedObject(PlaceableSurface _surface) {
        if (_surface.placePoint != null) {
            // Look at surface
            Vector3 lookDir = _surface.placePoint.position - player.transform.position;
            lookDir.y = 0f;
            player.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            Transform _obj = currentlyCarriedObj;
            DropCarriedObject();
            // Place on surface
            _surface.PlaceObject(_obj);
        }
    }

    public void SetDownCarriedObject() {
        player.anim.TriggerDropAnim();
    }

    public void UseCarriedObject() {
        if (currentlyCarriedObj)
            player.crafting.UseObject(currentlyCarriedObj);
    }
}
