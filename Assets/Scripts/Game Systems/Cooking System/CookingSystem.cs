using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    public static float cookingTempThreshold = 135f; // Food begins cooking when temperatures are above 135 degrees F
    private static float cookingSpeedModifier = 0.6f;

    public static float CalculateAddedCookingProgress(FoodItem _food) {
        return ((((_food.thermalBody.massTemp - cookingTempThreshold) * 1.2f) / _food.thermalBody.mass) * cookingSpeedModifier);
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
