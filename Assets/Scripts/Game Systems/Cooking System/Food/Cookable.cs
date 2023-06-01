using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoodItem))]
public class Cookable : MonoBehaviour
{
    // Settings
    public float cookingRequired = 100f;
    public float overcookBuffer = 100f;

    // States
    public float cookingProgress { get; private set; } = 0f;
    public float overcookProgress { get; private set; } = 0f;

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
        if (_cookTotal > cookingRequired) {
            cookingProgress = cookingRequired;
            overcookProgress = _cookTotal - cookingRequired;
        } else {
            cookingProgress = _cookTotal;
        }
        food.ui.cookDonenessSlider.SetValue(cookingProgress);
        food.ui.overcookSlider.SetValue(overcookProgress);
    }

    public void Cook(float _added) {
        if (cookingProgress == 0) {
            food.ui.cookDonenessSlider.InitializeValues(0, cookingRequired);
            food.ui.cookDonenessSlider.Show();
            food.ui.overcookSlider.InitializeValues(0, overcookBuffer);
            food.ui.overcookSlider.Show();
        } else {
            food.ui.cookDonenessSlider.TriggerSlider();
        }

        if (cookingProgress < cookingRequired) {
            cookingProgress += _added;
            if (cookingProgress > cookingRequired) cookingProgress = cookingRequired;
            food.ui.cookDonenessSlider.SetValue(cookingProgress);
        } else {
            if (overcookProgress < overcookBuffer) {
                overcookProgress += _added;
                if (overcookProgress > overcookBuffer) overcookProgress = overcookBuffer;
                food.ui.overcookSlider.SetValue(overcookProgress);
            } else return;
        }
    }
}
