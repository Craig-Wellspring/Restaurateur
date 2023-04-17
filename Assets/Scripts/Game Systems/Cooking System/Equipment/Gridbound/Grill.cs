using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grill : MonoBehaviour
{
    [SerializeField] private CookingArea cookingArea;
    [SerializeField] private float cookingTemp;

    private void Awake() {
        GetComponentInChildren<Toggle>().OnToggle += Toggle;
    }

    private void OnDestroy() {
        GetComponentInChildren<Toggle>().OnToggle -= Toggle;      
    }

    public void Toggle(object _sender, System.EventArgs _args) {
        Toggle.ToggleEventArgs args = (Toggle.ToggleEventArgs)_args;
        cookingArea.SetTargetTemp(args.isOn ? cookingTemp : GameManager.Master.airTemp);
    }
}
