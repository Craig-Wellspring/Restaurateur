using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThermalBody))]
public class DishManager : MonoBehaviour
{
    // States
    public float dishQualityTotal { get; private set; }
    public FlavorProfile flavorProfile = new();

    // Cache
    public ThermalBody thermalBody { get; private set; }
    public DishComponent mainComponent { get; private set; }
    public List<DishComponent> sideComponents { get; private set; } = new();
    public List<DishComponent> toppingComponents { get; private set; } = new();
    public DishComponent sauceComponent { get; private set; }
    public List<DishComponent> garnishComponents { get; private set; } = new();
    
    private void Awake() {
        thermalBody = GetComponent<ThermalBody>();

        foreach(Transform child in transform) {
            if (child.name.Contains("Main")) {
                mainComponent = child.GetComponent<DishComponent>();
                continue;
            }
            if (child.name.Contains("Side")) {
                sideComponents.Add(child.GetComponent<DishComponent>());
                continue;
            }
            if (child.name.Contains("Topping")) {
                toppingComponents.Add(child.GetComponent<DishComponent>());
                continue;
            }
            if (child.name.Contains("Sauce")) {
                sauceComponent = child.GetComponent<DishComponent>();
                continue;
            }
            if (child.name.Contains("Garnish")) {
                garnishComponents.Add(child.GetComponent<DishComponent>());
                continue;
            }
        }
    }

    private void SetMesh(Transform _targetComponent, Transform _sourceFood) {
        _targetComponent.GetComponent<MeshFilter>().sharedMesh = _sourceFood.GetComponent<MeshFilter>().sharedMesh;
        _targetComponent.GetComponent<MeshRenderer>().sharedMaterials = _sourceFood.GetComponent<MeshRenderer>().sharedMaterials;
    }

    public void PlateFood(Transform _sourceFood, DishComponent _targetComponent) {
        SetMesh(_targetComponent.transform, _sourceFood);

        _sourceFood.TryGetComponent<FoodItem>(out FoodItem _fItem);

        _targetComponent.foodType = _fItem.foodType;

        thermalBody.mass += _fItem.thermalBody.mass;
        dishQualityTotal += _fItem.quality;
        flavorProfile.AddProfile(_fItem.flavorProfile);
        
        // TODO: Adjust temperature
    }

    public float GetDishAverageQuality() {
        return dishQualityTotal / GetPlacedComponents().Count;
    }

    public Transform TryAddToPlate(FoodItem _food) {
        switch(_food.componentType) {
            case FoodItem.DishComponentType.Main:
                if (mainComponent.IsReset()) {
                    PlateFood(_food.transform, mainComponent);
                    // Dish's thermal conductivity is based on that of the main component
                    thermalBody.density = _food.thermalBody.density;
                    return mainComponent.transform;
                } else return null;

            case FoodItem.DishComponentType.Side:
                DishComponent _side = sideComponents.Find((c) => c.IsReset());
                if (_side != null) {
                    PlateFood(_food.transform, _side);
                    // If there is not yet a main component, base dish's thermal conductivity off of side
                    if (mainComponent.IsReset()) thermalBody.density = _food.thermalBody.density;
                    return _side.transform;
                } else return null;

            // Not possible to add the following without a main component
            case FoodItem.DishComponentType.Topping:
                DishComponent _topping = toppingComponents.Find((c) => c.IsReset());
                if (_topping != null && !mainComponent.IsReset()) {
                    PlateFood(_food.transform, _topping);
                    return _topping.transform;
                } else return null;

            case FoodItem.DishComponentType.Sauce:
                if (sauceComponent == null && !mainComponent.IsReset()) {
                    PlateFood(_food.transform, sauceComponent);
                    return sauceComponent.transform;
                } else return null;

            case FoodItem.DishComponentType.Garnish:
                DishComponent _garnish = garnishComponents.Find((c) => c.IsReset());
                if (_garnish != null && !mainComponent.IsReset()) {
                    PlateFood(_food.transform, _garnish);
                    return _garnish.transform;
                } else return null;

            default: return null;
        }
    }

    public List<DishComponent> GetAllComponents() {
        List<DishComponent> _list = new();

        _list.Add(mainComponent);
        sideComponents.ForEach((c) => { _list.Add(c); });
        toppingComponents.ForEach((c) => { _list.Add(c); });
        _list.Add(sauceComponent);
        garnishComponents.ForEach((c) => { _list.Add(c); });

        return _list;
    }

    public List<DishComponent> GetPlacedComponents() {
        return GetAllComponents().FindAll((c) => !c.IsReset());
    }

    public void ClearDish() {
        mainComponent = new();
        sideComponents.ForEach((c) => { c = new(); });
        toppingComponents.ForEach((c) => { c = new(); });
        sauceComponent = new();
        garnishComponents.ForEach((c) => { c = new(); });

        dishQualityTotal = 0f;
        flavorProfile = new();

        thermalBody.mass = 0f;
        thermalBody.density = 1f;
    }


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
