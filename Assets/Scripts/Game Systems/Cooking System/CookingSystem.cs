using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    public static float cookingTempThreshold = 135f; // Food begins cooking when temperatures are above 135 degrees F
    private static float cookingSpeedModifier = 0.6f;

    public static float CalculateAddedCookingProgress(FoodItem _food) {
        return Mathf.Clamp(((((_food.foodTemp - cookingTempThreshold) * 1.2f) / _food.mass) * cookingSpeedModifier), 0, 100);
    }

    public static float CalculateHeatChange(float _mass, float _thermalConductivity, float _subjectTemp, float _environsTemp) {
        return (0.005f / _mass) * -_thermalConductivity * ((_subjectTemp - _environsTemp) * 2);
    }
  
  // public enum UtensilTypes {
    // Knife,
    // Cleaver,
    // Whisk,
    // Spatula,
    // RollingPin,
    // Shredder,
    // Masher,
    // Ladle,
    // Sponge,
    // Thermometer,
    // KnifeSharpener
  // }
}
