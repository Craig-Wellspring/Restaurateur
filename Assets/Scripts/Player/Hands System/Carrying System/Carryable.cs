using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : Item, IClickable
{
    private void Awake() {
        base.LoadRefs();
    }

    public void Click(PlayerManager _player, int _L0orR1) {
      // Hands empty, object is in interact zone, pickup carryable object
      if (_player.hands.handsFree) {
          if (_player.interact.CanInteract(this.transform)) {
              _player.carry.LiftObject(this.transform);
              _player.hands.SetHandsFree(false);
              _player.hands.SetIsCarrying(true);
          }
          return;
      }

      // Clicked object being carried, set it down
      if (_player.carry.GetCarriedObject() == this.transform) {
          _player.carry.SetDownCarriedObject();
      }
      return;
    }
}
