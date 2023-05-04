using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(CanvasGroup))]
public class SliderController : MonoBehaviour
{
    // Events
    public event EventHandler OnTrigger;

    // Cache
    private Slider slider;
    private Image fill;
    private LayoutElement element;
    private CanvasGroup canvas;

    // Settings
    public float timeToHide = 3f;
    public Gradient fillGradient;

    // States
    private float timer = 0f;
    public bool isShowing { get; private set; } = false;


    private void Awake() {
        slider = GetComponent<Slider>();
        fill = slider.transform.Find("Fill").GetComponent<Image>();
        element = GetComponent<LayoutElement>();
        canvas = GetComponent<CanvasGroup>();
        if (!isShowing) Hide();
    }

    private void Update() {
        if (timer < timeToHide) {
            timer += Time.deltaTime;
        } else {
            if (isShowing) Hide();
        }        
    }

    public void InitializeValues(float _currentValue = 0, float _maxValue = 100) {
        SetValue(_currentValue);
        SetMaxValue(_maxValue);
    }

    public void TriggerSlider() {
        OnTrigger?.Invoke(this, EventArgs.Empty);

        if (isShowing) {
            ResetTimer();
        } else {
            Show();
        }
    }

    public void ResetTimer() {
        timer = 0f;
    }

    public void Hide() {
        element.ignoreLayout = true;
        canvas.alpha = 0;
        isShowing = false;
    }

    public void Show() {
        element.ignoreLayout = false;
        canvas.alpha = 1;
        isShowing = true;
        ResetTimer();
    }

    public void SetValue(float _value) {
        slider.value = _value;
    }

    public void SetMaxValue(float _max) {
        slider.maxValue = _max;
    }

    public void SetFillColor(Color _color) {
        fill.color = _color;
    }
}
