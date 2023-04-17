using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packable : MonoBehaviour
{
  public GridboundObject packableObj;
  private Dictionary<object, bool> locks = new Dictionary<object, bool>();

  public void AddLock(object _self) {
    locks.TryAdd(_self, false);
  }
  
  public void SetIsLocked(object _lock, bool _isLocked) {
    locks[_lock] = _isLocked;
  }

  public bool CanPack() {
    return !locks.ContainsValue(true);
  }
}
