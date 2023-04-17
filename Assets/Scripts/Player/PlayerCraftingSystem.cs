using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCraftingSystem : MonoBehaviour
{
    public event EventHandler CombineEvent;
    private PlayerManager player;
    

    private void Awake() {
        player = GetComponent<PlayerManager>();
    }

    public void CombineObjects(Transform _leftHandObj, Transform _rightHandObj) {
        if (_leftHandObj.TryGetComponent<ICombinable>(out ICombinable _LCombine) && _rightHandObj.TryGetComponent<ICombinable>(out ICombinable _RCombine)) {
            _LCombine.CombineWith(_RCombine);

            Debug.Log("Combine " + _leftHandObj.name + " and " + _rightHandObj.name);
            CombineEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public void UseObject(Transform _obj) {
        if (_obj.TryGetComponent<IUsable>(out IUsable usable)) {
            usable.Use(player);
            Debug.Log("Used " + _obj.name);
        }    
    }

    public void UseTogether(Transform _obj1, Transform _obj2) {
        Debug.Log("Used "+ _obj1.name + " and " + _obj2.name + " together");        
    }
}
