using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodUIManager : MonoBehaviour
{
    public SliderController temperatureGauge;
    public TextMeshProUGUI tempNumDisplay;
    public SliderController contaminationSlider;
    public SliderController cookDonenessSlider;
    public SliderController overcookSlider;
    public SliderController cutProgressSlider;


    private BillboardController canvasController;

    private void Awake() {
        canvasController = GetComponent<BillboardController>();

        temperatureGauge.OnTrigger += TriggerUI;
    }

    private void OnDestroy() {
        temperatureGauge.OnTrigger -= TriggerUI;
    }

    private void TriggerUI(object _sender, System.EventArgs _args) {
        if (canvasController.isShowing) {
            canvasController.ResetTimer();
        } else {
            canvasController.Show();
        }
    }
}
