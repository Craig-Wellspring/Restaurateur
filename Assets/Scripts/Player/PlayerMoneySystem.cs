using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoneySystem : MonoBehaviour
{
    // Cache
    PlayerManager player;

    // States
    public float currentMoney = 1500;
    
    private void Awake() {
        player = GetComponent<PlayerManager>();        
    }

    public void AddMoney(float _amount) {
        currentMoney += _amount;
        player.ui.money.SetMoneyCounter(currentMoney);
    }

    public bool TakeMoney(float _amount) {
        if (currentMoney - _amount >= 0) {
            currentMoney -= _amount;
            player.ui.money.SetMoneyCounter(currentMoney);
            return true;
        } else return false;
    }
}
