using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardManager : MonoBehaviour
{
    private PlayerManager player;
    private PlayerInputActions actions;
    public Vector3 movementInput { get; private set; }
    public int cameraRotationInput { get; private set; }
    public bool inverseCamRotation = false;

    private void Awake() {
        player = GetComponent<PlayerManager>();
        actions = new PlayerInputActions();
        actions.Enable();
    }

    private void Update() {
        ReadMovementInputs();
    }

    // Movement controls
    private void ReadMovementInputs() {
        Vector2 rawMovementInput = actions.Player.Move.ReadValue<Vector2>();

        Vector3 worldUp = player.mainCam.transform.forward;
        worldUp.y = 0f;
        worldUp.Normalize();

        Vector3 worldRight = player.mainCam.transform.right;
        worldRight.y = 0f;
        worldRight.Normalize();

        Vector3 desiredMoveDirection = worldUp * rawMovementInput.y + worldRight * rawMovementInput.x;
        movementInput = desiredMoveDirection * Mathf.Clamp(rawMovementInput.magnitude, -1f, 1f) * player.movement.moveSpeed;
    }

    // Camera controls

    private void OnRotateCamera(InputValue value) {
        float rawCamRotateInput = value.Get<float>();
        cameraRotationInput = inverseCamRotation ? -(int)rawCamRotateInput : (int)rawCamRotateInput;
    }

    private void OnZoomCamera(InputValue value) {
        float rawScrollInput = value.Get<float>();
        float normalizedScrollInput = rawScrollInput / 120;
        player.movement.ZoomCamera(normalizedScrollInput);
    }

    // Keyboard Controls
    public void OnJump() { // Space
        player.movement.Jump();
    }

    public void OnToggleBlueprintMode() { // Tab
        player.building.ToggleBlueprintMode();
    }

    public void OnGrab() { // G
        if (Keyboard.current.shiftKey.isPressed) {
            if (!player.building.blueprintModeOn)
                player.building.ToggleBlueprintMode();
            player.ghostController.ChangeSelector(GhostController.SelectorType.Boxing);
            return;
        }
        if (player.building.blueprintModeOn) {
            player.building.BoxGrab();
        } else {
            player.hands.HandsGrab();
        }
    }

    public void OnUse() { // F
        if (player.building.blueprintModeOn) {
            // Holding packed Carrybox - Switch to placement mode
            if (player.hands.isCarrying && player.carry.GetCarriedObject().TryGetComponent<PackingBox>(out PackingBox _box)) {
                _box.Use(player);
                return;
            }
        } else {
            player.hands.HandsUse();
        }
    }

    public void OnInteract() { // C
        if (player.building.blueprintModeOn) {
        } else {
            player.interact.Interact();
        }
    }

    public void OnSwitch() { // X
        // Blueprint Mode - Select Demolition tool
        if (player.building.blueprintModeOn) {
            player.ghostController.ChangeSelector(GhostController.SelectorType.Demolish);
        } else {
            // Hold shift to switch, otherwise combine
            if (Keyboard.current.shiftKey.isPressed)
                player.hands.HandsSwitchObjects();
            else 
                player.hands.HandsCombine();
        }
    }

    public void OnRelease() { // Z
        // Blueprint Mode - Select Inquiry tool
        if (player.building.blueprintModeOn) {
            player.ghostController.ChangeSelector(GhostController.SelectorType.Inquiry);
        } else {
            if (Keyboard.current.shiftKey.isPressed)
                player.hands.HandsRelease(true); // Drop only left
            else 
                player.hands.HandsRelease();
        }
    }

    public void OnRotate() { // R
        if (Keyboard.current.shiftKey.isPressed)
            player.ghostController.CounterRotateSelector();
        else 
            player.ghostController.RotateSelector();
    }

    public void OnCheckTime() { // T
        GameManager.Master.time.GetCurrentTime();
    }

    // Blueprint Hotbar keys
    public void OnQuick1() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(0);
        } else {
        }
    }

    public void OnQuick2() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(1);
        } else {
        }
    }

    public void OnQuick3() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(2);
        } else {
        }
    }

    public void OnQuick4() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(3);
        } else {
        }
    }

    public void OnQuick5() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(4);
        } else {
        }
    }

    public void OnQuick6() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(5);
        } else {
        }
    }

    public void OnQuick7() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(6);
        } else {
        }
    }

    public void OnQuick8() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(7);
        } else {
        }
    }

    public void OnQuick9() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(8);
        } else {
        }
    }

    public void OnQuick10() {
        if (player.building.blueprintModeOn) {
            player.ghostController.SelectBlueprintPlacement(9);
        } else {
        }
    }
}
