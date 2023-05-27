using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingArea : MonoBehaviour
{
    // Material density of the cooking medium. Higher densities take longer to heat up and transfer heat to food faster
    private enum MediumType {
        Air = 1,
        Oil = 9,
        Water = 10,
        Metal = 80,
    }

    [SerializeField] private MediumType mediumDensity;
    private float currentTemp = 70f;
    private float targetTemp = 70f;

    private void Awake() {
        TimeManager.OnSecondChange += StabilizeTemp;
        SetTargetTemp(GameManager.Master.airTemp);
    }

    private void OnDestroy() {
        TimeManager.OnSecondChange -= StabilizeTemp;        
    }

    private void StabilizeTemp(object _sender, System.EventArgs _args) {
        currentTemp = Mathf.Lerp(currentTemp, targetTemp, (int)mediumDensity * 0.0005f);
    }

    public void SetTargetTemp(float _temp) {
        targetTemp = _temp;
    }

    private void OnTriggerEnter(Collider _other) {
        if (_other.transform.TryGetComponent<ThermalBody>(out ThermalBody _tBody)) {
            _tBody.SetEnvironsTemp(currentTemp);
        }
    }

    private void OnTriggerStay(Collider _other) {
        if (_other.transform.TryGetComponent<ThermalBody>(out ThermalBody _tBody)) {
            _tBody.SetEnvironsTemp(currentTemp);
        }        
    }

    private void OnTriggerExit(Collider _other) {
        if (_other.transform.TryGetComponent<ThermalBody>(out ThermalBody _tBody)) {
            _tBody.SetEnvironsTemp(GameManager.Master.airTemp);
        }        
    }
}
