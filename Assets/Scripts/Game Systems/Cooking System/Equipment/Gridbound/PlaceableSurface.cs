using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;

public class PlaceableSurface : MonoBehaviour, IClickable
{
    public Transform placePoint;
    public event EventHandler ObjectPlaced;

    public void PlaceObject(Transform _obj, Vector3? _specificPos = null) {
        void Placed() { ObjectPlaced?.Invoke(this, EventArgs.Empty); }
        StartCoroutine(UtilsClass.LerpPosition(_obj, _specificPos == null ? placePoint.position : (Vector3)_specificPos, Quaternion.LookRotation(_obj.forward, Vector3.up), 0.5f, Placed));
    }

    public void Click(PlayerManager _player, int _L0orR1) {
        // Player is holding or carrying something
        if (!_player.hands.handsFree) {
            // Carrying object, set down
            if (_player.hands.isCarrying && _player.interact.CanInteract(this.transform)) {
                Transform carriedObj = _player.carry.GetCarriedObject();
                _player.carry.DropCarriedObject();
                PlaceObject(carriedObj, placePoint == null ? _player.mouse.mouseoverWorldPos : placePoint.position);
                return;
            }

            // Not carrying, check hands
            HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
            Transform handObj = _hand.GetHeldObject();

            // Place held object
            if (handObj != null) {
                _hand.ReleaseHeldObject();
                PlaceObject(handObj, _player.mouse.mouseoverWorldPos);
                return;
            }

            return;
        }
    }
}
