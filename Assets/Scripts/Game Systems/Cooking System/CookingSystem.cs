using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    public static float minDoneness = -100f;
    public static float maxDoneness = 100f;
    public static float cookingTempThreshold = 135f; // Food begins cooking when temperatures are above 135 degrees F

    public static float CalculateCooking(FoodItem food) {
        float addedDoneness = 0f;
        if (food.foodTemp >= cookingTempThreshold) {
            addedDoneness = ((food.foodTemp - cookingTempThreshold) * 1.1f) * (1 / food.densityMod);
        }
        return addedDoneness;
    }
    
    public static float Cook(FoodItem _food, float _currentDoneness) {
        float newDoneness = _currentDoneness;
        float added = CalculateCooking(_food);
        if (_currentDoneness + added <= maxDoneness) {
            newDoneness += added;
        } else {
            newDoneness = maxDoneness;
        }

        return newDoneness;
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
