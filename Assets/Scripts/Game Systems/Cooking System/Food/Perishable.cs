using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;

[RequireComponent(typeof(FoodItem))]
public class Perishable : MonoBehaviour
{
    // States
    public float contamination { get; private set; } = 0f;

    // Settings
    [SerializeField] private float contaminationRate = 0.1f;
    [SerializeField] private float groundContamRate = 100f;

    // Cache
    private FoodItem food;
    private int ticks = 0;
    public static float maxContam { get; private set; } = 1000f;

    private void Awake() {
        TimeManager.OnTick += TickCounter;
        food = GetComponent<FoodItem>();
    }

    private void OnDestroy() {
        TimeManager.OnTick -= TickCounter;
    }

    private void TickCounter(object _sender, System.EventArgs _args) {
        ticks++;

        if (ticks == 5) {
            ticks -= 5;
            ContaminationCheck();
        }
    }

    private void ContaminationCheck() {
        // On floor, heavily contaminate
        if (UtilsClass.IsOnGround(transform.position)) {
            food.SetTemp(Mathf.Lerp(food.foodTemp, GameManager.Master.airTemp, 1f));            
            contamination = Mathf.Clamp(contamination + groundContamRate, 0, maxContam);
            return;
        }

        SetContamination(contamination + CalculateContamination(food.foodTemp, contaminationRate));
    }

    public static float CalculateContamination(float _currentTemp, float _contamRate = 1f) {
        float contamTick = 0;
        switch (_currentTemp) {
            // Over cooking temperature range, sanitize
            case > 135: contamTick = -(_currentTemp - 135); break;
            // In critical temperature range, contaminate
            case > 40: contamTick = _contamRate; break;
            // Under freezing temperature range, sanitize
            case < 33: contamTick = -(_contamRate * (33 - _currentTemp)); break;
        }

        return contamTick;
    }

    public void SetContamination(float _target) {
        contamination = Mathf.Clamp(_target, 0, maxContam);
    }
}
