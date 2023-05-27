using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seasoning : MonoBehaviour
{
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

[System.Serializable]
public class Flavor {
    public FlavorType type;
    public float intensity;

    public Flavor(FlavorType _type, float _intensity) {
        type = _type;
        intensity = _intensity;
    }

    public void AddFlavor(float _intensity) {
        intensity += _intensity;
    }
}

[System.Serializable]
public class FlavorProfile {
    public List<Flavor> profile;

    public FlavorProfile() {
        profile = new();

        profile.Add(new Flavor(FlavorType.Sweet, 0));
        profile.Add(new Flavor(FlavorType.Sour, 0));
        profile.Add(new Flavor(FlavorType.Spicy, 0));
        profile.Add(new Flavor(FlavorType.Salty, 0));
        profile.Add(new Flavor(FlavorType.Savory, 0));
        profile.Add(new Flavor(FlavorType.Pungent, 0));
        profile.Add(new Flavor(FlavorType.Bitter, 0));
        profile.Add(new Flavor(FlavorType.Menth, 0));
    }

    public void AddProfile(FlavorProfile _targetProfile) {
        for (int i = 0; i < profile.Count; i++ ) {
            profile[i].intensity += _targetProfile.profile[i].intensity;
        }
    }
}