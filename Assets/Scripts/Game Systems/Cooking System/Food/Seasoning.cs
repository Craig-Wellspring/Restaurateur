using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seasoning : MonoBehaviour
{
  [System.Serializable]
  public class Flavor {
      public string name;
      public Seasoning.FlavorType type;
      public float intensity;

      public Flavor(Seasoning.FlavorType _type, float _intensity) {
          name = _type.ToString();
          type = _type;
          intensity = _intensity;
      }

      public void AddFlavor(float _intensity) {
        intensity += _intensity;
      }
  }

  public enum FlavorType {
    Neutral, // Nothing
    Sweet, // Sugar
    Salty, // Salt
    Sour, // Acid
    Spicy, // Chili
    Pungent, // Onion
    Savory, // Mushroom
    Bitter, // Coffee
    Menth, // Mint
  }

  public FlavorType type;
  public int intensity = 1;
  public int remaining = 1;
  public bool destroyOnEmpty = true;

  public bool Consume(int _quantity = 1) {
    if (remaining - _quantity >= 0) {
      remaining -= _quantity;
      if (remaining == 0 && destroyOnEmpty)
        Destroy(this.gameObject);

      if (TryGetComponent<Cuttable>(out Cuttable _cut)) {
        _cut.PreventCutting();
      }

      return true;
    }
    return false;
  }
}
