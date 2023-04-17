using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombinable
{
  public void CombineWith(ICombinable _target);
}
