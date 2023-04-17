using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour, IClickable
{
    public class ToggleEventArgs : EventArgs {
        public bool isOn;
    }
    public event EventHandler OnToggle;
    public bool _isOn { get; private set; } = false;

    private void DoToggle() {
        _isOn = !_isOn;
        OnToggle?.Invoke(this, new ToggleEventArgs{ isOn = _isOn} );
    }

    public void Click(PlayerManager _player, int _L0orR1) {
        DoToggle();
    }
}
