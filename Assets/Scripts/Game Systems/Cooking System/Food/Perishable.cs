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
    [SerializeField] private float maxContam = 100f;
    [SerializeField] private float groundContamRate = 5f;

    [SerializeField] private static float contaminationRate = 0.1f;

    // Cache
    private FoodItem food;

    private void Awake() {
        food = GetComponent<FoodItem>();

        TimeManager.OnSecondChange += ContaminationCheck;
    }

    private void OnDestroy() {
        TimeManager.OnSecondChange -= ContaminationCheck;
    }

    private void ContaminationCheck(object _sender, System.EventArgs _args) {
        float addedContam = UtilsClass.IsOnGround(transform.position) ? groundContamRate : CalculateContamination(food.thermalBody.massTemp);

        if (addedContam != 0) AddContamination(addedContam);
    }

    public static float CalculateContamination(float _temp) {
        float addedContam = ((Mathf.Pow(10f, (-Mathf.Pow((_temp - 90f), 2) / 8000f))) * 2) - 1;
        if (addedContam < 0) addedContam = addedContam *= 50f;
        return addedContam * contaminationRate;
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
