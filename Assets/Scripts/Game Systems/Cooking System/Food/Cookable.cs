using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoodItem))]
public class Cookable : MonoBehaviour
{
    // States
    public float cookedProgress { get; private set; } = 0f;
    public float overcookedProgress { get; private set; } = 0f;

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
        float _added = CookingSystem.CalculateAddedCookingProgress(food);
        if (_added > 0) {
            Cook(_added);
        }
    }

    public void SetCookProgress(float _cookTotal) {
        if (_cookTotal > 100) {
            cookedProgress = 100;
            overcookedProgress = _cookTotal - 100;
        } else {
            cookedProgress = _cookTotal;
        }
        food.ui.cookDonenessSlider.SetValue(cookedProgress);
        food.ui.overcookSlider.SetValue(overcookedProgress);
    }

    public void Cook(float _added) {
        if (cookedProgress == 0) {
            food.ui.cookDonenessSlider.InitializeValues();
            food.ui.cookDonenessSlider.Show();
            food.ui.overcookSlider.InitializeValues();
            food.ui.overcookSlider.Show();
        } else {
            food.ui.cookDonenessSlider.TriggerSlider();
        }

        if (cookedProgress < 100) {
            cookedProgress += _added;
            if (cookedProgress > 100) cookedProgress = 100;
            food.ui.cookDonenessSlider.SetValue(cookedProgress);
        } else {
            if (overcookedProgress < 100) {
                overcookedProgress += _added;
                if (overcookedProgress > 100) overcookedProgress = 100;
                food.ui.overcookSlider.SetValue(overcookedProgress);
            } else return;
        }
    }
}
