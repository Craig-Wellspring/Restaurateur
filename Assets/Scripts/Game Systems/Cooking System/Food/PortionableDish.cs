using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortionableDish : MonoBehaviour
{
  public int portions { get; private set; }
  public int maxPortions { get; private set; }

  public bool AddPortion(int _count = 1) {
    if (portions + _count <= maxPortions) {
      portions += _count;
      return true;
    } else {
      return false;
    }
  }

  public bool RemovePortion(int _count = 1) {
    if (portions -_count >= 0) {
      portions -= _count;
      return true;
    } else {
      return false;
    }
  }
}
