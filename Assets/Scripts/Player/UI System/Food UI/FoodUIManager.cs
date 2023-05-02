using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodUIManager : MonoBehaviour
{
    public SliderController temperatureSlider;
    public SliderController cookDonenessSlider;
    public SliderController overcookSlider;
    public SliderController cutProgressSlider;


    private BillboardController canvasController;

    private void Awake() {
        canvasController = GetComponent<BillboardController>();

        temperatureSlider.OnTrigger += TriggerUI;
    }

    private void OnDestroy() {
        temperatureSlider.OnTrigger -= TriggerUI;
    }

    private void TriggerUI(object _sender, System.EventArgs _args) {
        if (canvasController.isShowing) {
            canvasController.ResetTimer();
        } else {
            canvasController.Show();
        }
    }

    public void UpdateTempFillColor(float _temp) {
        temperatureSlider.SetFillColor(GameManager.Master.tempGradient.Evaluate(Mathf.Clamp(_temp / 100, 0, 1)));
    }
}
