using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalBody : MonoBehaviour
{
    public event EventHandler TempTick;

    // Settings
    public float mass = 1f;
    [Tooltip("Density of material. Higher conductivity changes temperature more quickly. 1 is equally as dense as water. 0 is a perfect thermal insulator.")]
    public float thermalConductivity = 1f;

    // States
    public float massTemp { get; private set; } = 70f;
    public float environsTemp { get; private set; } = 70f;

    private void Awake() {
        TimeManager.OnTick += StabilizeTemp;
    }

    private void OnDestroy() {
        TimeManager.OnTick -= StabilizeTemp;
    }
    
    
    private void StabilizeTemp(object _sender, System.EventArgs _args) {
        if (mass > 0) massTemp += CalculateHeatChange(mass, thermalConductivity, massTemp, environsTemp);
        TempTick?.Invoke(this, EventArgs.Empty);
    }

    public static float CalculateHeatChange(float _mass, float _thermalConductivity, float _subjectTemp, float _environsTemp) {
        return (0.005f / _mass) * -_thermalConductivity * ((_subjectTemp - _environsTemp) * 2);
    }

    public void SetTemp(float _newTemp) {
        massTemp = _newTemp;
    }
    
    public void SetEnvironsTemp(float _newTemp) {
        environsTemp = _newTemp;
    }
}
