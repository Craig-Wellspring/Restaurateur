using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;

public class PlayerHandsSystem : MonoBehaviour
{
    // Settings
    public float interactRange = 4f;
    [Tooltip("Time in seconds it takes for objects to travel to and from the hands")]
    public float pickupSpeed = 1f;

    [Tooltip("If mouse button held beyond this threshold in seconds, consider button held")]
    [SerializeField] private float clickThreshold = .250f;
    [SerializeField] private bool dragLineEnabled = true;
    [SerializeField] private LayerMask lineMask;

    // Cache
    private PlayerManager player;

    // States
    private float clickTime = 0f;
    public bool heldActionActive { get; private set; } = false;
    private Rigidbody dragTarget;
    private float dragDistToCam;
    private HoldablesContainer openStorage;

    public bool handsFree { get; private set; } = false;
    public bool isCarrying { get; private set; } = false;
    public bool isProcessing { get; private set; } = false;
    private Func<Transform, bool> Process;
    private Transform processTarget;
    
    // Switches
    public void SetHandsFree(bool _is) {
        handsFree = _is;
    }
    public void SetIsCarrying(bool _is) {
        isCarrying = _is;
    }

    private void Awake() {
        player = GetComponent<PlayerManager>();
    }

    //// Mouse Commands
    private void Start() {
        player.mouse.LeftMouseDown += MouseDown;
        player.mouse.LeftMouseUp += MouseUp;

        player.mouse.RightMouseDown += MouseDown;
        player.mouse.RightMouseUp += MouseUp;

        player.building.OnEnterBlueprintMode += ResetHandActions;
    }

    private void OnDestroy() {
        player.mouse.LeftMouseDown -= MouseDown;
        player.mouse.LeftMouseUp -= MouseUp;

        player.mouse.RightMouseDown -= MouseDown;
        player.mouse.RightMouseUp -= MouseUp;

        player.building.OnEnterBlueprintMode -= ResetHandActions;
    }

    private void Update() {
        SetHandsFree(!player.leftHand.GetHeldObject() && !player.rightHand.GetHeldObject() && !isCarrying);

        if (!player.building.blueprintModeOn) {   
            // Increment click time
            if (player.mouse.isLeftMouseHeld || player.mouse.isRightMouseHeld) {
                clickTime += Time.deltaTime;
                if (processTarget != null && player.interact.CanInteract(processTarget) && Process(processTarget)) {
                    isProcessing = true;
                } else {
                    isProcessing = false;
                }

                // Click is being held
                if (clickTime > clickThreshold) {
                    if (!(heldActionActive && !isProcessing)) {
                        if (player.mouse.isLeftMouseHeld)
                            ActivateHeldAction(0);
                        if (player.mouse.isRightMouseHeld)
                            ActivateHeldAction(1);
                    }
                }
            }
        }
    }

    private void FixedUpdate() {
        if (heldActionActive) {
            if (player.mouse.isLeftMouseHeld)
                MouseHeldActionFixed(0);
            if (player.mouse.isRightMouseHeld)
                MouseHeldActionFixed(1);
        }
    }

    private void LateUpdate() {
        if (heldActionActive) {
            if (player.mouse.isLeftMouseHeld)
                MouseHeldActionLate(0);
            if (player.mouse.isRightMouseHeld)
                MouseHeldActionLate(1);
        }
    }

    private void MouseDown(object sender, System.EventArgs e) {
        MouseManager.MouseClickParams click = (MouseManager.MouseClickParams)e;
        if (!player.building.blueprintModeOn) {   
            DownHandAction(click.L0orR1);
        }
    }

    private void MouseUp(object sender, System.EventArgs e) {
        MouseManager.MouseClickParams click = (MouseManager.MouseClickParams)e;
        if (!player.building.blueprintModeOn) {
            // Click was not held, fire normal click
            if (clickTime < clickThreshold) {
                ClickHandAction(click.L0orR1);
            } else {
                UpHandAction(click.L0orR1);
            }
            ResetHandActions();

            if (click.L0orR1 == 0) {
                if (!player.mouse.isLeftMouseHeld)
                    clickTime = 0f;
            } else {
                if (!player.mouse.isRightMouseHeld)
                    clickTime = 0f;
            }
        }
    }

    private void ResetHandActions(object sender = null, System.EventArgs e = null) {
        // Reset click
        heldActionActive = false;
        
        // Disable ghosts
        if (player.ghostController.selector.gameObject.activeSelf)
            player.ghostController.DeactivateSelectorGhost();

        // Disable dragging
        if (dragTarget != null) {
            player.dragLine.SetPositions(new [] { player.transform.position, player.transform.position });
            player.dragLine.enabled = false;
            dragTarget = null;
        }

        // Stop processing
        if (isProcessing) {
            Process = null;
            isProcessing = false;
            processTarget = null;
        }

        // Close open inventories
        if (openStorage) {
            player.ui.inventory.CloseInventory(openStorage);
            openStorage = null;
        }

        // Show Cursor
        if (!player.building.blueprintModeOn)
            player.mouse.ToggleCursorVisible(true);
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(this.transform.position, interactRange);    
    }

    public bool CanReachPosition(Vector3 _pos) {
        return !(UtilsClass.IsWallBetween(transform.position + Vector3.up, _pos) || Vector3.Distance(transform.position + Vector3.up, _pos) > interactRange);
    }

    // Begin evaluating the click for draggable object, utensil, or dish
    private void DownHandAction(int _L0orR1) {
        // Not in Blueprint Mode, check range and LoS
        if (!isCarrying && CanReachPosition(player.mouse.mouseoverWorldPos)) {
            // Check Hand
            HandHold _hand = _L0orR1 == 0 ? player.leftHand : player.rightHand;
            Transform handObj = _hand.GetHeldObject();

            if (handObj == null) { // Hand empty
                // Check for Draggable
                if (player.mouse.mouseoverObject?.GetComponent<Draggable>() != null && player.mouse.mouseoverObject.TryGetComponent<Rigidbody>(out Rigidbody _clickRB)) {
                    // Register distance between camera and object when clicked
                    dragDistToCam = Vector3.Distance(_clickRB.position, player.mainCam.transform.position);
                    dragTarget = _clickRB;
                    return;
                }
                // Check for Storage
                if (openStorage == null /* && player.mouse.mouseoverGridObj != null */ && player.mouse.mouseoverObject.TryGetComponent<HoldablesContainer>(out HoldablesContainer _container)) {
                    player.ui.inventory.PopulateInventoryGrid(_container);
                    openStorage = _container;
                    return;
                }

            } else { // Something in hand
                if (player.mouse.mouseoverObject != null) {
                    if (handObj.TryGetComponent<IUtensil>(out IUtensil _utensil) && player.interact.CanInteract(player.mouse.mouseoverObject)) {
                        if (_utensil.Use(player.mouse.mouseoverObject)) {
                            player.mouse.ToggleCursorVisible(false);
                            processTarget = player.mouse.mouseoverObject;
                            Process = (target) => _utensil.Use(processTarget);
                        }
                        return;
                    }
                }
            }
        }
    }

    private void UpHandAction(int _L0orR1) {
        // Pickup mouseover grid inventory object
        if (player.ui.inventory?.mouseoverInventorySlotIndex != null && openStorage != null) {
            // Check hands
            HandHold _hand = _L0orR1 == 0 ? player.leftHand : player.rightHand;
            Transform handObj = _hand.GetHeldObject();
            // Hand empty - pick up item
            if (handObj == null && player.ui.inventory.mouseoverInventorySlotIndex != null) {
                int _objIndex = (int)player.ui.inventory.mouseoverInventorySlotIndex;
                openStorage.TakeItem(_hand, _objIndex);
            }
        }
    }

    // Click held, respond to dragging, processing, browsing
    private void ActivateHeldAction(int _L0orR1) { // Start mouseHeldDown actions
        heldActionActive = true;
        // Check Hand
        HandHold _hand = _L0orR1 == 0 ? player.leftHand : player.rightHand;
        Transform handObj = _hand.GetHeldObject();

        if (handObj != null && processTarget == null) {
            player.ghostController.ActivateSelectorGhost(_hand);
            return;
        }

        if (dragTarget != null) {
            player.mouse.ToggleCursorVisible(false);
            if (dragLineEnabled)
                player.dragLine.enabled = true;
            return;
        }

        if (handObj == null && openStorage != null) {
            player.ui.inventory.OpenInventory(openStorage);
        }
    }

    private void MouseHeldActionLate(int _L0orR1) { // LateUpdate mouseHeldDown action
        if (openStorage != null) {
            // Keep inventory grid centered over inventory object
            player.ui.inventory.transform.position = player.mainCam.WorldToScreenPoint(openStorage.spawnPoint.position);
            // Close inventory if too far away
            if (!CanReachPosition(openStorage.spawnPoint.position)) {
                player.ui.inventory.CloseInventory(openStorage);
                openStorage = null;
            }
        }
    }

    private void MouseHeldActionFixed(int _L0orR1) { // FixedUpdate mouseHeldDown actions
        if (dragTarget != null) {
            // Maintain distance between camera and object when dragging
            Vector3 dragPos = Input.mousePosition;
            dragPos.z = dragDistToCam;
            dragPos = player.mainCam.ScreenToWorldPoint(dragPos);
            
            // Keep object inside of interact range
            if (Vector3.Distance(dragPos, player.transform.position) > interactRange) {
                Vector3 dirToDragPos = dragPos - player.transform.position;
                dragPos = player.transform.position + (dirToDragPos.normalized * interactRange);
            }
            
            // Prevent object from going around walls
            if (Scripts.Utils.UtilsClass.IsWallBetween(player.transform.position + Vector3.up, dragPos)) {
                Physics.Raycast(player.transform.position + Vector3.up, dragPos, out RaycastHit wallHit, interactRange, LayerMask.GetMask("Wall"));
                dragPos = wallHit.point;
            }

            // Move object toward cursor
            dragTarget.velocity = (dragPos - dragTarget.position) * 10;

            // Draw line between object and surface beneath it
            Physics.Raycast(dragTarget.position + (dragTarget.transform.up * 0.001f), Vector3.down, out RaycastHit dragHit, 6f, lineMask);
            if (dragHit.point == Vector3.zero)
                dragHit.point = dragTarget.position;
            player.dragLine.SetPositions(new [] { dragTarget.position, dragHit.point });
        }
    }

    // Normal click (down and back up quickly), respond to click
    private void ClickHandAction(int _L0orR1) {
        // Not in Blueprint Mode, check range and LoS
        if (CanReachPosition(player.mouse.mouseoverWorldPos)) {
            IClickable[] _clickables = Scripts.Utils.UtilsClass.GetInterfaces<IClickable>(player.mouse.mouseoverObject.gameObject);
            foreach (IClickable _clicked in _clickables) {
                _clicked.Click(player, _L0orR1);
            }
        }
    }

    //// Keyboard Commands

    // While no hands active, attempt to pick up objects in reach. With hand ghost active, place object in world.
    public void HandsGrab() {
        // Hands free, look for carryable object
        if (handsFree) {
            List<Transform> carryableObjects = player.interact.GetInteractables();
            Transform pickupTarget = null;
            foreach(Transform _nearObj in carryableObjects) {
                if (_nearObj.GetComponent<Carryable>() && CanReachPosition(_nearObj.transform.position)){
                    pickupTarget = _nearObj;
                    break;
                }
            }

            // Found carryable object, pick it up
            if (pickupTarget != null) {
                player.carry.LiftObject(pickupTarget);
                return;
            }
        }

        // No hands active - Pick up surrounding objects
        if (!player.mouse.isLeftMouseHeld && !player.mouse.isRightMouseHeld && !isCarrying) {
            Collider[] nearbyObjects = Physics.OverlapSphere(player.transform.position, interactRange, LayerMask.GetMask("HoldableObject"));
            foreach (Collider _holdObj in nearbyObjects) {
                if (CanReachPosition(_holdObj.transform.position)) {
                    if (player.leftHand.GetHeldObject() == null) {
                        player.leftHand.HoldObject(_holdObj.transform);
                        break;
                    }
                    if (player.rightHand.GetHeldObject() == null) {
                        player.rightHand.HoldObject(_holdObj.transform);
                        break;
                    }
                }
            }
            return;
        }

        // Holding packed Carrybox - Switch to placement mode
        Transform _carryObj = player.carry.GetCarriedObject();
        if (isCarrying && _carryObj && _carryObj.TryGetComponent<PackingBox>(out PackingBox _box)) {
            _box.Use(player);
            return;
        }

        // Object ghost active - Place object
        if (CanReachPosition(player.mouse.mouseoverWorldPos)) {
            if (player.ghostController.selector.gameObject.activeSelf) {
                if (player.mouse.isLeftMouseHeld && player.leftHand.GetHeldObject() != null)
                    player.leftHand.PlaceHeldObject(player.ghostController.selector.position, player.ghostController.selector.rotation);
                else if (player.mouse.isRightMouseHeld && player.rightHand.GetHeldObject() != null)
                    player.rightHand.PlaceHeldObject(player.ghostController.selector.position, player.ghostController.selector.rotation);
                player.ghostController.DeactivateSelectorGhost();
            }
        }

        // Dragging holdable object - Grab it
        if (dragTarget != null && heldActionActive) {
            if (dragTarget.GetComponent<HoldableItem>() != null) {
                if (player.mouse.isLeftMouseHeld && player.leftHand.GetHeldObject() == null)
                    player.leftHand.HoldObject(dragTarget.transform);
                else if (player.mouse.isRightMouseHeld && player.rightHand.GetHeldObject() == null)
                    player.rightHand.HoldObject(dragTarget.transform);
                player.dragLine.enabled = false;
                dragTarget = null;
            }
        }
    }


    // Consume/Use objects in hands. [Left, Right, Both(Simultaneous), Both(Combined), Carried, Dragging]
    public void HandsUse() {
        // Use carried object
        if (isCarrying) {
            player.carry.UseCarriedObject();
            return;
        }

        // Use held objects, default left hand first
        Transform leftHandObj = player.leftHand.GetHeldObject();
        Transform rightHandObj = player.rightHand.GetHeldObject();

        // Left hand active, holding or dragging
        if (player.mouse.isLeftMouseHeld) {
            if (leftHandObj == null) {
                if (dragTarget != null)
                    player.crafting.UseObject(dragTarget.transform);
            } else {
                player.leftHand.UseHeldObject();
            }
        }

        // Right hand active, holding or dragging
        if (player.mouse.isRightMouseHeld) {
            if (rightHandObj == null) {
                if (dragTarget != null)
                    player.crafting.UseObject(dragTarget.transform);
            } else {
                player.rightHand.UseHeldObject();
            }
        }

        // Neither hand active
        if (!player.mouse.isLeftMouseHeld && !player.mouse.isRightMouseHeld) {
            // Only holding left item
            if (leftHandObj != null && rightHandObj == null) {
                player.leftHand.UseHeldObject();
            }

            // Only holding right item
            if (leftHandObj == null && rightHandObj != null) {
                player.rightHand.UseHeldObject();
            }

            // Holding in both (Combined usage)
            if (leftHandObj != null && rightHandObj != null) {
                player.crafting.UseTogether(leftHandObj, rightHandObj);
            }

            // Holding in neither
            return;
        }
    }

    public void HandsRelease(bool _shiftHeld = false) {
        // Drop carried object
        if (isCarrying) {
            // If holding shift, just release
            if (_shiftHeld) {
                player.carry.DropCarriedObject();
                return;
            }

            // Check for nearby surfaces, place on surface if found
            List<Transform> nearbySurfaces = player.interact.GetInteractables();
            foreach(Transform _surface in nearbySurfaces) {
                if (_surface.TryGetComponent<PlaceableSurface>(out PlaceableSurface _ps) && CanReachPosition(_ps.transform.position)) {
                    player.carry.PlaceCarriedObject(_ps);
                    return;
                }
            }

            // No surface, place it on the ground
            player.carry.SetDownCarriedObject();
            return;
        }
        
        // Drop only left
        if (player.mouse.isLeftMouseHeld || _shiftHeld) {
            player.leftHand.ReleaseHeldObject();
        }

        // Drop only right
        if (player.mouse.isRightMouseHeld) {
            player.rightHand.ReleaseHeldObject();
        }

        // Drop both
        if (!player.mouse.isLeftMouseHeld && !player.mouse.isRightMouseHeld && !_shiftHeld) {
            player.leftHand.ReleaseHeldObject();
            player.rightHand.ReleaseHeldObject();
        }
    }


    // Holding System - Each hand can hold an object
    public void HandsSwitchObjects() {
        Transform leftHandObj = player.leftHand.GetHeldObject();
        Transform rightHandObj = player.rightHand.GetHeldObject();

        // Left hand full, right hand empty
        if (leftHandObj != null && rightHandObj == null) {
            player.rightHand.HoldObject(leftHandObj);
            return;
        }

        // Right hand full, left hand empty
        if (rightHandObj != null && leftHandObj == null) {
            player.leftHand.HoldObject(rightHandObj);
            return;
        }

        // Both hands full
        if (leftHandObj != null && rightHandObj != null) {
            player.leftHand.ReleaseHeldObject();
            player.rightHand.ReleaseHeldObject();
            player.leftHand.HoldObject(rightHandObj);
            player.rightHand.HoldObject(leftHandObj);
            return;
        }

        // Both hands empty
        if (leftHandObj == null && rightHandObj == null) {
            return;
        }
    }

    public void HandsCombine() {
        Transform leftHandObj = player.leftHand.GetHeldObject();
        Transform rightHandObj = player.rightHand.GetHeldObject();

        // If holding two objects, combine them
        if (leftHandObj != null && rightHandObj != null) {
            player.crafting.CombineObjects(leftHandObj, rightHandObj);
        } else {
            Debug.Log("Both hands must be full to combine objects");
        }
    }
}
