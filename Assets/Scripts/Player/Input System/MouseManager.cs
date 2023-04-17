using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    // Settings
    [SerializeField] private Texture2D arrowTexture;
    [SerializeField] private Texture2D pointerTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    // Events
    public event EventHandler LeftMouseDown;
    public event EventHandler LeftMouseUp;
    public bool isLeftMouseHeld { get; private set; }

    public event EventHandler RightMouseDown;
    public event EventHandler RightMouseUp;
    public bool isRightMouseHeld { get; private set; }


    // Cursor information
    public Vector3 mouseGroundPos { get; private set; }
    
    public GridSector<BuildingGridTile> mouseoverGrid { get; private set; }
    public Vector3 mouseGridSnappedGroundPos { get; private set; }
    public Vector2Int mouseGridCoords { get; private set; }
    public BuildingGridTile mouseoverGridObj { get; private set; }
    public GridboundItem mouseoverPlacedGridboundObj { get; private set; }

    public LayerMask mouseoverObjLayers;
    public Transform mouseoverObject { get; private set; }
    public Vector3 mouseoverWorldPos { get; private set; }

    
    // Cache
    private PlayerManager player;

    public class MouseClickParams : EventArgs {
        public int L0orR1;
        public MouseClickParams(int _L0orR1) {
            L0orR1 = _L0orR1;
        }
    }
    private void Awake() {
        player = GetComponent<PlayerManager>();
    }

    private void OnEnable() {
        SwitchToArrow();        
    }

    private void OnDisable() {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    private void Update() {
        MouseEvents();
        UpdateCursorInfo();
    }

    public void SwitchToArrow() {
        Cursor.SetCursor(arrowTexture, hotSpot, cursorMode);
    }

    public void SwitchToPointer() {
        Cursor.SetCursor(pointerTexture, hotSpot, cursorMode);
    }

    public void ToggleCursorVisible(bool _doShow) {
        Cursor.visible = _doShow;
    }

    private void MouseEvents() {
        // Left Mouse Down
        if (Mouse.current.leftButton.wasPressedThisFrame)
            LeftMouseDown?.Invoke(this, new MouseClickParams(0));

        // Left Mouse Held
        isLeftMouseHeld = Mouse.current.leftButton.isPressed;

        // Left Mouse Up
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            LeftMouseUp?.Invoke(this, new MouseClickParams(0));

        // Right Mouse Down
        if (Mouse.current.rightButton.wasPressedThisFrame)
            RightMouseDown?.Invoke(this, new MouseClickParams(1));

        // Right Mouse Held
        isRightMouseHeld = Mouse.current.rightButton.isPressed;
        
        // Right Mouse Up
        if (Mouse.current.rightButton.wasReleasedThisFrame)
            RightMouseUp?.Invoke(this, new MouseClickParams(1));
    }

    private void UpdateCursorInfo() {
        mouseGroundPos = GetMouseGroundPosition();
        mouseoverGrid = GameManager.Master.grid.GetGridFromPosition(mouseGroundPos);
        mouseGridCoords = GetMouseGridCoordinates();
        mouseGridSnappedGroundPos = GetMouseGridSnappedGroundPosition();
        mouseoverGridObj = mouseoverGrid?.GetGridObject(mouseGroundPos);
        mouseoverPlacedGridboundObj = mouseoverGridObj?.GetPlacedObject();
        (mouseoverObject, mouseoverWorldPos) = GetMouseoverWorldPoint();
    }

    private Vector3 GetMouseGroundPosition()
    {
        Ray ray = player.mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            return hit.point;
        else
            return Vector3.zero;
    }

    private Vector2Int GetMouseGridCoordinates() {
        Vector2Int _coords = Vector2Int.zero;
        if (mouseoverGrid != null) {
            mouseoverGrid.GetXZ(mouseGroundPos, out int x, out int z);
            _coords = new Vector2Int(x, z);
        }
        return _coords;
    }

    private Vector3 GetMouseGridSnappedGroundPosition()
    {
        if (mouseoverGrid != null) {
            mouseoverGrid.GetXZ(mouseGroundPos, out int x, out int z);

            if (player.ghostController.currentSelectorType != GhostController.SelectorType.None)
            {
                Vector2Int rotationOffset = player.ghostController.currentGridObj != null ? player.ghostController.currentGridObj.GetRotationOffset(player.ghostController.currentSelectorDir) : new Vector2Int(0, 0);
                Vector3 placedObjectWorldPosition =
                    mouseoverGrid.GetWorldPosition(x, z) +
                    new Vector3(rotationOffset.x, 0, rotationOffset.y) *
                    GameManager.Master.grid.cellSize;
                return placedObjectWorldPosition;
            }
        }
        
        return mouseGroundPos;
    }

    private (Transform obj, Vector3 pos) GetMouseoverWorldPoint() {
        Ray ray = player.mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, mouseoverObjLayers, QueryTriggerInteraction.Ignore))
            return (hit.transform, hit.point);
        else
            return (null, Vector3.zero);
    }
}
