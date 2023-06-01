using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalBody : MonoBehaviour
{
    public event EventHandler OnTempChange;

    // Settings
    [Tooltip("Total mass in kilograms.")]
    public float mass = 1f;
    [Tooltip("Density of material in g/mL. Higher density increases thermal conductivity which means the material changes temperature more quickly. 1 is equally as dense as water. 0 is a perfect thermal insulator. Steel is 8. Aluminum is 2.7.")]
    public float density = 1f;

    // States
    [Tooltip("Temperature in degrees F.")]
    public float massTemp { get; private set; } = 70f;
    public float environsTemp { get; private set; } = 70f;

    private void Awake() {
        TimeManager.OnTick += StabilizeTemp;
    }

    private void OnDestroy() {
        TimeManager.OnTick -= StabilizeTemp;
    }
    
    
    private void StabilizeTemp(object _sender, System.EventArgs _args) {
        if (mass > 0) {
            float _heatChange = CalculateHeatChange(mass, density, massTemp, environsTemp);
            if (_heatChange != 0) {
                SetTemp(massTemp + _heatChange);
                OnTempChange?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public static float CalculateHeatChange(float _mass, float _density, float _subjectTemp, float _environsTemp) {
        return (0.005f / _mass) * -_density * ((_subjectTemp - _environsTemp) * 2);
    }

    public void SetTemp(float _newTemp) {
        massTemp = _newTemp;
    }
    
    public void SetEnvironsTemp(float _newTemp) {
        environsTemp = _newTemp;
    }
}
