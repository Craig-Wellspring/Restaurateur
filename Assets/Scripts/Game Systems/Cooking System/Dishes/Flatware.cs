using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flatware : DishItem
{
  public FlatwareType flatwareType;
  
  private void Awake() {
      base.LoadRefs();
  }
}

public enum FlatwareType {
    ButterKnife,
    SteakKnife,
    Fork,
    Spoon,
    Chopsticks
}
