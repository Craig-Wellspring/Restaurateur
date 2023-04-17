using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serveware : DishItem
{
  public ServewareType servewareType;
  public int portionsRemaining;
  
  private void Awake() {
      base.LoadRefs();
  }
}

public enum ServewareType {
    Container,
    Platter,
    ServingBowl,
    Pot,
    Board,
}

