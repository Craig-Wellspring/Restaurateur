using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRotate : MonoBehaviour
{
    public Vector3 rotation = new Vector3(0, 0, -90);

    private void Awake() {
        transform.root.GetComponentInChildren<Toggle>().OnToggle += ToggleOn;
    }

    private void OnDestroy() {
        transform.root.GetComponentInChildren<Toggle>().OnToggle -= ToggleOn;      
    }

    private void ToggleOn(object _sender, System.EventArgs _args) {
        Toggle.ToggleEventArgs args = (Toggle.ToggleEventArgs)_args;
        this.transform.localRotation = args.isOn ? Quaternion.Euler(rotation) : Quaternion.identity;
    }
}
