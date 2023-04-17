using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinnerware : DishItem
{
  public DinnerwareType dinnerwareType;
  
  private void Awake() {
      base.LoadRefs();
  }
}

public enum DinnerwareType {
    SmallPlate,
    LargePlate,
    SmallBowl,
    LargeBowl,
}
  
