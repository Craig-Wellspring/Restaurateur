using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooler : MonoBehaviour
{
    // Settings
    [Range(-10, 70), SerializeField] private float setTemp = 32f;
    [SerializeField] private float coolingRate = 0.001f;
    [SerializeField] private float warmingRate = 0.1f;

    // States
    private float targetTemp;
    private float currentTemp;
    private bool powerOn;

    // Cache
    private DigitalDisplay display;
    private List<HoldablesContainer> compartments = new List<HoldablesContainer>();

    private void Awake() {
        display = GetComponentInChildren<DigitalDisplay>();
        foreach(HoldablesContainer _container in GetComponentsInChildren<HoldablesContainer>()) {
            compartments.Add(_container);
        }
        TimeManager.OnTenSeconds += AdjustCompartmentTemps;
        GetComponentInChildren<Toggle>().OnToggle += TogglePower;
    }
    
    private void OnDestroy() {
        TimeManager.OnTenSeconds -= AdjustCompartmentTemps;
        GetComponentInChildren<Toggle>().OnToggle -= TogglePower;
    }

    private void Start() {
        targetTemp = powerOn ? setTemp : GameManager.Master.airTemp;
        currentTemp = targetTemp;
        display.SetDisplay(Mathf.RoundToInt(currentTemp));
    }

    public void SetTargetTemp(float _temp) {
        setTemp = _temp;
        if (powerOn)
            targetTemp = setTemp;
    }

    private void TogglePower(object _sender, System.EventArgs _args) {
        Toggle.ToggleEventArgs args = (Toggle.ToggleEventArgs)_args;
        targetTemp = args.isOn ? setTemp : GameManager.Master.airTemp;
        powerOn = args.isOn;
    }

    private void AdjustCompartmentTemps(object _sender, System.EventArgs _args) {
        currentTemp = Mathf.Lerp(currentTemp, targetTemp, targetTemp > setTemp ? coolingRate : warmingRate);
        display.SetDisplay(Mathf.RoundToInt(currentTemp));

        foreach(HoldablesContainer _compartment in compartments) {
            _compartment.ambientTemp = currentTemp;
        }
    }
}
