using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : BillboardController
{
    private Slider slider;

    private void Awake() {
        slider = GetComponentInChildren<Slider>(true);
    }

    public void SetValue(float _value) {
        slider.value = _value;
    }

    public void SetMaxValue(float _max) {
        slider.maxValue = _max;
    }
}
