using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinnerware : DishItem
{
    // Settings
    public DinnerwareType dinnerwareType;
    [SerializeField] private int maxSides = 3;
    [SerializeField] private int maxToppings = 3;
    [SerializeField] private int maxGarnishes = 3;

    // States
    public PlatedDish platedDish = new();
    public DishManager dishManager { get; private set; }
  
    private void Awake() {
        base.LoadRefs();

        dishManager = transform.GetComponentInChildren<DishManager>();
    }

    public bool TryAddToPlate(FoodItem _food) {
        switch(_food.componentType) {
            case FoodItem.DishComponentType.Main:
                if (platedDish.mainComponent == null) {
                    platedDish.mainComponent = _food;
                    PlateFood(_food.transform, dishManager.mainPos);
                    return true;
                } else return false;

            case FoodItem.DishComponentType.Side:
                if (platedDish.sideComponents.Count < maxSides) {
                    platedDish.sideComponents.Add(_food);
                    return true;
                } else return false;

            case FoodItem.DishComponentType.Topping:
                if (platedDish.toppingComponents.Count < maxToppings) {
                    platedDish.toppingComponents.Add(_food);
                    return true;
                } else return false;

            case FoodItem.DishComponentType.Sauce:
                if (platedDish.sauceComponent == null) {
                    platedDish.sauceComponent = _food;
                    return true;
                } else return false;

            case FoodItem.DishComponentType.Garnish:
                if (platedDish.garnishComponents.Count < maxGarnishes) {
                    platedDish.garnishComponents.Add(_food);
                    return true;
                } else return false;

            default: return false;
        }
    }
    
    public void ClearPlate() {
        platedDish = new();
    }

    private void PlateFood(Transform _foodTransform, Transform _platingPos) {

    }
}

public enum DinnerwareType {
    SmallPlate,
    LargePlate,
    SmallBowl,
    LargeBowl,
}

[System.Serializable]
public class PlatedDish {
    public FoodItem mainComponent;
    public List<FoodItem> sideComponents;
    public List<FoodItem> toppingComponents;
    public FoodItem sauceComponent;
    public List<FoodItem> garnishComponents;


    public string ListIngredients() {
        string ingList = "";
        ingList += mainComponent.name + ", ";
        sideComponents.ForEach((side) => { ingList += side.name + ", "; });
        if (toppingComponents.Count > 0) {
            ingList += "topped with ";
            toppingComponents.ForEach((topping) => { ingList += topping.name + ", "; });
        }
        if (sauceComponent != null) {
            ingList += "with " + sauceComponent.name;
        }
        if (garnishComponents.Count > 0) {
            ingList += "garnished with ";
            garnishComponents.ForEach((garnish) => { ingList += garnish.name + ", "; } );

        }

        return ingList;
    }
}
