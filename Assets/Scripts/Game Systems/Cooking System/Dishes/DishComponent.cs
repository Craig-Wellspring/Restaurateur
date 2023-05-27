using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FoodItem;

public class DishComponent : MonoBehaviour
{
    public FoodType foodType;

    public bool IsReset() {
        return foodType == FoodType.None;
    }

    public void Reset() {
        foodType = FoodType.None;
    }
}
