using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLights : MonoBehaviour
{
    private Light offLight;
    private Light onLight;

    private void Awake() {
        transform.root.GetComponentInChildren<Toggle>().OnToggle += Toggle;

        offLight = transform.Find("Off Light").GetComponent<Light>();
        onLight = transform.Find("On Light").GetComponent<Light>();
    }

    private void OnDestroy() {
        transform.root.GetComponentInChildren<Toggle>().OnToggle -= Toggle;      
    }

    private void Toggle(object _sender, System.EventArgs _args) {
        Toggle.ToggleEventArgs args = (Toggle.ToggleEventArgs)_args;
        offLight.enabled = !args.isOn;
        onLight.enabled = args.isOn;
    }
}
