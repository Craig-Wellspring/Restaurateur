using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoodItem))]
public class Cookable : MonoBehaviour
{
    // States
    [Tooltip("Scale from Raw (-100) to Burnt (100), 0 is perfect but taste may vary")]
    public float doneness { get; private set; } = -100f;

    // Cache
    private FoodItem food;

    private void Awake() {
        food = GetComponent<FoodItem>();
        TimeManager.OnTick += TryCook;
    }

    private void OnDestroy() {
        TimeManager.OnTick -= TryCook;
    }

    private void TryCook(object _sender, System.EventArgs _args) {
        SetDoneness(CookingSystem.Cook(food, doneness));
    }


    public void SetDoneness(float _new) {
        doneness = _new;
    }
}
