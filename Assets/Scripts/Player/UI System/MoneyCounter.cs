using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyCounter : MonoBehaviour
{
    private PlayerManager player;
    private TextMeshProUGUI counter;
    private CanvasGroup controller;

    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
        counter = GetComponentInChildren<TextMeshProUGUI>();
        controller = GetComponent<CanvasGroup>();
    }

    private void OnEnable() {
        SetMoneyCounter(player.money.currentMoney);
    }

    public void SetMoneyCounter(float _amount) {
        counter.text = string.Format("{0:C}", _amount).Remove(0, 1);
    }

    public void Show() {
        controller.alpha = 1;
    }

    public void Hide() {
        controller.alpha = 0;
    }
}
