using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildingSystem : MonoBehaviour
{
    // Cache
    private PlayerManager player;
    [SerializeField] private Transform carrybox;

    // Events
    public event EventHandler OnEnterBlueprintMode;

    // States
    public List<GridboundObject> placeableBlueprintsList; // Placeable Blueprint Inventory
    public bool blueprintModeOn { get; private set; } = false;

    private void Awake() {
        player = GetComponent<PlayerManager>();
    }

    private void Start() {
        player.mouse.LeftMouseDown += OnLeftClick;
    }

    private void OnDestroy() {
        player.mouse.LeftMouseDown -= OnLeftClick;
    }


    private void OnLeftClick(object sender, System.EventArgs e) {
        // Left Click in Blueprint Mode
        if (blueprintModeOn)
        {
            switch (player.ghostController.currentSelectorType) {
                case GhostController.SelectorType.None: return;

                // Blueprint Selector - Build Blueprint
                case GhostController.SelectorType.Blueprint:
                    PlaceGridObject();
                break;

                // Object Selector - Place Object
                case GhostController.SelectorType.Object:
                    PlaceGridObject();
                break;

                // Demolition Selector - Destroy Grid Object
                case GhostController.SelectorType.Demolish:
                    player.mouse.mouseoverPlacedGridboundObj?.DestroySelf();
                break;

                // Inquiry Selector - Inquire Grid Object Data
                case GhostController.SelectorType.Inquiry:
                    string inquiryString = "";

                    if (player.mouse.mouseoverPlacedGridboundObj != null) {
                        string objectName = player.mouse.mouseoverPlacedGridboundObj.gridboundObject.prefab.name;
                        Vector3 objectDimensions = new Vector3(player.mouse.mouseoverPlacedGridboundObj.gridboundObject.gridWidth * GameManager.Master.grid.cellSize, player.mouse.mouseoverPlacedGridboundObj.gridboundObject.verticalMeters, player.mouse.mouseoverPlacedGridboundObj.gridboundObject.gridHeight * GameManager.Master.grid.cellSize);
                        string objectData = "Name: " + objectName + ", Footprint: (" + player.mouse.mouseoverPlacedGridboundObj.gridboundObject.gridWidth + " x " + player.mouse.mouseoverPlacedGridboundObj.gridboundObject.gridHeight + "), Dimensions(m): (" + objectDimensions.x + ", " + objectDimensions.y + ", " + objectDimensions.z + "), ";

                        inquiryString += objectData;
                    }

                    if (player.mouse.mouseoverGridObj != null)
                        inquiryString += player.mouse.mouseoverGridObj;

                    Debug.Log(inquiryString);
                break;

                // Boxing Selector - Pack Object in a box
                case GhostController.SelectorType.Boxing:
                    Transform targetObj = player.mouse.mouseoverPlacedGridboundObj?.transform;
                    if (targetObj && Vector3.Distance(player.transform.position, targetObj.position) <= player.hands.interactRange && player.mouse.mouseoverPlacedGridboundObj.TryGetComponent<Packable>(out Packable pck) && pck.CanPack()) {
                        // Remove object
                        player.mouse.mouseoverPlacedGridboundObj.DestroySelf();
                        // Spawn carrybox containing object
                        Transform _newBox = Instantiate(carrybox, player.mouse.mouseGroundPos, targetObj.transform.rotation);
                        _newBox.name = carrybox.name;
                        _newBox.GetComponent<PackingBox>().Pack(targetObj);
                    }
                break;
            }
        }
    }

    public bool PlaceGridObject() {
        // Footprint vacant for building
        if (player.hands.CanReachPosition(player.ghostController.transform.position)) {
            if (player.ghostController.IsFootprintAvailable()) {
                Vector2Int rotationOffset = player.ghostController.currentGridObj.GetRotationOffset(player.ghostController.currentSelectorDir);
                Vector3 objOriginWorldPosition = player.mouse.mouseoverGrid.GetWorldPosition(player.mouse.mouseGridCoords.x, player.mouse.mouseGridCoords.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * GameManager.Master.grid.cellSize;

                // Create object
                GridboundItem placedObject = GridboundItem.Create(objOriginWorldPosition, player.mouse.mouseGridCoords, player.ghostController.currentSelectorDir, player.ghostController.currentGridObj);

                // Occupy building grid coordinates with object
                List<Vector2Int> gridPositionList = player.ghostController.currentGridObj.GetGridPositionList(player.mouse.mouseGridCoords, player.ghostController.currentSelectorDir);
                foreach (Vector2Int _gridPosition in gridPositionList)
                {
                    player.mouse.mouseoverGrid.GetGridObject(_gridPosition.x, _gridPosition.y).SetPlacedObject(placedObject);
                }

                if (player.ghostController.currentSelectorType == GhostController.SelectorType.Object) {
                    ToggleBlueprintMode();
                    if (player.hands.isCarrying && player.carry.GetCarriedObject().TryGetComponent<PackingBox>(out PackingBox _box)) {
                        player.carry.DropCarriedObject();
                        _box.DestroySelf();
                    }
                }

                return true;
            } else { // Footprint occupied
                Debug.Log("Position occupied or not on a grid");
                return false;
            }
        } else { // Out of range
            Debug.Log("Position too far away or obstructed.");
            return false;
        }
    }

    public void BoxGrab() {
        // If carrying a packed box, open placement mode
        if (player.hands.isCarrying) {
            Transform _carryBox = player.carry.GetCarriedObject();
            if (_carryBox != null && _carryBox.TryGetComponent<PackingBox>(out PackingBox _box)) {
                // Place Object if ghost active
                if (blueprintModeOn && player.ghostController.currentSelectorType == GhostController.SelectorType.Object) {
                    PlaceGridObject();
                } else {
                    // Open Object Placement mode if not active
                    _box.Use(player);
                }        
            }
            return;
        }

        List<Transform> nearbyObjects = player.interact.GetInteractables();

        // If hands free
        if (player.hands.handsFree) {
            // Try to pick up nearby box
            foreach(Transform _obj in nearbyObjects) {
                if (_obj.GetComponent<PackingBox>() && player.hands.CanReachPosition(_obj.transform.position)) {
                    player.carry.LiftObject(_obj);
                    return;
                }
            }         
        }

        // Can't pick up nearby box, try to box a nearby packable object
        foreach(Transform _obj in nearbyObjects) {
            if (_obj.TryGetComponent<Packable>(out Packable _pck) && _pck.CanPack() && player.hands.CanReachPosition(_obj.transform.position)) {
                _obj.TryGetComponent<GridboundItem>(out GridboundItem _placedObj);
                // Spawn carrybox containing object
                Transform _newBox = Instantiate(carrybox, _placedObj.GetObjectCenterWorldPos(), _obj.rotation);
                _newBox.name = carrybox.name;
                _newBox.GetComponent<PackingBox>().Pack(_obj);
                _placedObj?.DestroySelf();
                return;
            }
        }
    }

    public void ToggleBlueprintMode() {
        if (blueprintModeOn) {
            // Exit
            player.ghostController.ResetSelector();
            blueprintModeOn = false;
            player.mouse.ToggleCursorVisible(true);
            player.ui.money.Hide();

            foreach(GridSector<BuildingGridTile> _grid in GameManager.Master.grid.buildingGrids) {
                _grid.ShowVisualizer(false);
            }
        } else {
            // Enter
            player.ghostController.transform.position = player.mouse.mouseoverWorldPos;
            blueprintModeOn = true;
            OnEnterBlueprintMode?.Invoke(this, EventArgs.Empty);
            player.ghostController.ChangeSelector(GhostController.SelectorType.Inquiry);
            player.mouse.ToggleCursorVisible(false);
            player.ui.money.Show();

            foreach(GridSector<BuildingGridTile> _grid in GameManager.Master.grid.buildingGrids) {
                _grid.ShowVisualizer(true);
            }
        }
    }
}

