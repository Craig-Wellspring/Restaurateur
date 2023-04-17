using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drinkware : DishItem
{
  public DrinkwareType drinkwareType;

  private void Awake() {
      base.LoadRefs();
  }
}

public enum DrinkwareType {
    Waterglass,
    Juiceglass,
    Wineglass,
    Beerglass,
    Mug,
}
