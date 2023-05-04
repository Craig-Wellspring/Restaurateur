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
    [SerializeField] private static float contaminationRate = 0.1f;
    [SerializeField] private float groundContamRate = 5f;

    // Cache
    private FoodItem food;
    public static float maxContam { get; private set; } = 100f;

    private void Awake() {
        food = GetComponent<FoodItem>();

        TimeManager.OnSecondChange += ContaminationCheck;
    }

    private void OnDestroy() {
        TimeManager.OnSecondChange -= ContaminationCheck;
    }

    private void ContaminationCheck(object _sender, System.EventArgs _args) {
        float addedContam = UtilsClass.IsOnGround(transform.position) ? groundContamRate : CalculateContamination(food.foodTemp);

        if (addedContam != 0) AddContamination(addedContam);
    }

    public static float CalculateContamination(float _temp) {
        float addedContam = 0;
        switch (_temp) {
            // Over cooking temperature range, sanitize
            case > 135: addedContam = -(_temp - 135) * contaminationRate; break;
            // In critical temperature range, contaminate
            case > 40: addedContam = (_temp - 40) * contaminationRate; break;
            // Under freezing temperature range, sanitize
            case < 33: addedContam = -(33 - _temp) * contaminationRate; break;
        }

        return addedContam;
    }

    public void AddContamination(float _added) {
        contamination = Mathf.Clamp(contamination + _added, 0, maxContam);

        float contamPercent = Mathf.Clamp(contamination / maxContam, 0, 1);
        food.ui.contaminationSlider.SetFillColor(food.ui.contaminationSlider.fillGradient.Evaluate(contamPercent));
        food.ui.contaminationSlider.SetValue(contamPercent);
        if (contamination != 0 && contamination != maxContam) food.ui.contaminationSlider.TriggerSlider();
    }

    public void SetContamination(float _target) {
        contamination = Mathf.Clamp(_target, 0, maxContam);
    }
}
