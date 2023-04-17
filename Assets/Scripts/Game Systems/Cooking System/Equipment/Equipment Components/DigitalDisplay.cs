using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitalDisplay : MonoBehaviour
{
    private TextMeshPro display;

    private void Awake() {
        display = GetComponentInChildren<TextMeshPro>();

        transform.root.GetComponentInChildren<Toggle>().OnToggle += TogglePower;
    }

    private void OnDestroy() {
        transform.root.GetComponentInChildren<Toggle>().OnToggle -= TogglePower;        
    }

    private void TogglePower(object _sender, System.EventArgs _args) {
        Toggle.ToggleEventArgs args = (Toggle.ToggleEventArgs)_args;
        display.color = args.isOn ? Color.green : Color.red;
    }

    public void SetDisplay(int _number) {
        display.text = _number.ToString();
    }
}
