using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    // Settings
    [SerializeField] private Mesh selectorModel;

        // Colors
        public Color validObjPlacementColor;
        public Color invalidObjPlacementColor;
        [SerializeField] private Color blueprintPlacementColor;
        [SerializeField] private Color demolishSelectorColor;
        [SerializeField] private Color demolishSelectedColor;
        [SerializeField] private Color inquirySelectorColor;
        [SerializeField] private Color inquirySelectedColor;
        [SerializeField] private Color boxingSelectorColor;
        [SerializeField] private Color boxingSelectedColor;

    // Cache
    private PlayerManager player;
    public Transform selector { get; private set; }
    private MeshFilter selectorMesh;
    private MeshRenderer selectorRenderer;
    private Transform pointer;

    // States
    public SelectorType currentSelectorType { get; private set; } = SelectorType.None;
    public Dir currentSelectorDir { get; private set; } = Dir.Down; // Direction orientation of blueprint ghost
    public GridboundObject currentGridObj { get; private set; } = null; // Currently selected Blueprint object
    public HoldableItem currentHoldableObj { get; private set; } = null; // Currently held object

    public enum SelectorType {
        None = 0,

        // Mesh types
        Holdable = 10,
        Blueprint = 11,
        Object = 12,

        // Arrow types
        Demolish = 20,
        Inquiry = 21,
        Boxing = 22,
    }

    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
        selector = transform.GetChild(0);
        selectorMesh = selector.GetComponent<MeshFilter>();
        selectorRenderer = selector.GetComponent<MeshRenderer>();
        pointer = selector.GetChild(0);
    }

    private void Start() {
        UpdateSelectorModel();
        transform.SetParent(GameManager.Master.transform);
    }

    private void LateUpdate() {
        if (selector.gameObject.activeSelf) {
            Vector3 targetPosition;
            Quaternion targetRotation = GetSelectorRotation();
            switch(currentSelectorType) {
                case SelectorType.Holdable:
                    targetPosition = player.mouse.mouseoverWorldPos;
                    // Turn towards player
                    Quaternion toPlayerRotation = Quaternion.LookRotation(player.transform.position - this.transform.position, Vector3.up);
                    // Always world up
                    toPlayerRotation = Quaternion.Euler(0, toPlayerRotation.eulerAngles.y, 0);
                    // Adjust for selector rotation
                    targetRotation *= toPlayerRotation;
                break;
                default:
                    targetPosition = player.mouse.mouseGridSnappedGroundPos;
                    // Find Object on grid coordinates
                    GridboundObject mouseoverObject = player.mouse.mouseoverPlacedGridboundObj?.gridboundObject;
                    // Hover ghost over target object if current selector is an Arrow type selector
                    if (mouseoverObject != null && (int)currentSelectorType >= 20)
                        targetPosition.y += mouseoverObject.verticalMeters;
                break;
            }

            // Color ghost
            UpdateSelectorColor();

            // Move ghost to target location and rotation
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    private void UpdateSelectorColor() {
        Color _clr = Color.clear;
        switch(currentSelectorType) {
            default:
            case SelectorType.Holdable:
                _clr = player.hands.CanReachPosition(player.mouse.mouseoverWorldPos) ? validObjPlacementColor : invalidObjPlacementColor;
            break;

            case SelectorType.Blueprint:
                _clr = IsFootprintAvailable() ? blueprintPlacementColor : invalidObjPlacementColor;
            break;

            case SelectorType.Object:
                _clr = IsFootprintAvailable() ? validObjPlacementColor : invalidObjPlacementColor;
            break;

            case SelectorType.Demolish:
                if (player.mouse.mouseoverGrid != null) {
                    if (IsFootprintAvailable()) {
                        _clr = demolishSelectorColor;
                    } else {
                        _clr = demolishSelectedColor;
                    }
                } else {
                    _clr = inquirySelectorColor;
                }
            break;

            case SelectorType.Inquiry:
                if (player.mouse.mouseoverGrid != null) {
                    if (IsFootprintAvailable()) {
                        _clr = inquirySelectorColor;
                    } else {
                        _clr = inquirySelectedColor;
                    }
                } else {
                    _clr = invalidObjPlacementColor;
                }
            break;

            case SelectorType.Boxing:
                if (player.mouse.mouseoverGrid != null) {
                    Packable packable = player.mouse.mouseoverPlacedGridboundObj?.GetComponent<Packable>();
                    if (!IsFootprintAvailable() && packable != null && packable.CanPack() && player.hands.CanReachPosition(player.mouse.mouseGridSnappedGroundPos)) {
                        _clr = boxingSelectedColor;
                    } else {
                        _clr = boxingSelectorColor;
                    }
                } else {
                    _clr = inquirySelectorColor;
                }
            break;
        }
        selectorRenderer.sharedMaterial.color = _clr;
    }

    public void UpdateSelectorModel() {
        switch (currentSelectorType) {
            default:
            case SelectorType.None:
                SetSelectorMesh(false, false);
            break;

            case SelectorType.Holdable:
                SetSelectorMesh(true, false, currentHoldableObj?.GetComponent<MeshFilter>().sharedMesh);
            break;

            case SelectorType.Blueprint:
                SetSelectorMesh(true, true, currentGridObj?.prefab.GetComponent<MeshFilter>().sharedMesh);
            break;
            
            case SelectorType.Object:
                MeshFilter[] meshFilters = currentGridObj?.prefab.GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combineArray = new CombineInstance[meshFilters.Length];
                int i = 0;
                while (i < meshFilters.Length) {
                    combineArray[i].mesh = meshFilters[i].sharedMesh;
                    combineArray[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    i++;
                }
                Mesh combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combineArray);
                SetSelectorMesh(true, true, combinedMesh);
            break;

            case SelectorType.Demolish:
                SetSelectorMesh(true, false, selectorModel);
            break;

            case SelectorType.Inquiry:
                SetSelectorMesh(true, false, selectorModel);
            break;

            case SelectorType.Boxing:
                SetSelectorMesh(true, false, selectorModel);
            break;
        }
    }

    private void SetSelectorMesh(bool _showSelector, bool _showPointer, Mesh _mesh = null) {
        selectorMesh.sharedMesh = _mesh;
        selector.gameObject.SetActive(_showSelector);

        pointer.gameObject.SetActive(_showPointer);
        if (_showPointer)
            pointer.localPosition = new Vector3(currentGridObj.gridWidth * GameManager.Master.grid.cellSize / 2, currentGridObj.verticalMeters / 2, 0);
    }

    public void ActivateSelectorGhost(HandHold _handObj) {
        player.mouse.ToggleCursorVisible(false);
        // Update mesh
        ChangeSelector(SelectorType.Holdable, _handObj.GetHeldObject().GetComponent<HoldableItem>());
        // Activate ghost
        transform.position = player.mouse.mouseoverWorldPos;
        selector.gameObject.SetActive(true);
    }

    public void DeactivateSelectorGhost() {
        if (!player.building.blueprintModeOn)
            player.mouse.ToggleCursorVisible(true);

        selector.transform.localPosition = Vector3.zero;
        selectorMesh.sharedMesh = null;
        selector.gameObject.SetActive(false);
    }

    public bool IsFootprintAvailable() {
        // Can't reach or mouseover occupied
        if (player.mouse.mouseoverPlacedGridboundObj != null || !player.hands.CanReachPosition(player.mouse.mouseGroundPos)) {
            return false;
        }

        // Calculate footprint of object to be placed
        if (player.mouse.mouseoverGrid != null) {
            List<Vector2Int> gridPositionList = currentGridObj != null ? currentGridObj.GetGridPositionList(player.mouse.mouseGridCoords, currentSelectorDir) : new List<Vector2Int>{ player.mouse.mouseGridCoords };

            // Check footprint grid coords occupancy
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                BuildingGridTile _tile = player.mouse.mouseoverGrid.GetGridObject(gridPosition.x, gridPosition.y);
                if (_tile?.GetGrid() == null || !_tile.CanBuild())
                {
                    // Grid square occupied
                    return false;
                }
            }
        } else {
            // No grid
            return false;
        }

        // Available
        return true;
    }

    public void ResetSelector() {
        currentSelectorType = GhostController.SelectorType.None;
        currentGridObj = null;    
        UpdateSelectorModel();    
    }

    public void SelectBlueprintPlacement(int _index) {
        if (_index < player.building.placeableBlueprintsList.Count) {
            ChangeSelector(GhostController.SelectorType.Blueprint, player.building.placeableBlueprintsList[_index]);
        }
    }

    public void ChangeSelector(SelectorType _type, object _obj = null) {

        switch (_type) {
            default:
            case SelectorType.None:
                currentGridObj = null;
                currentHoldableObj = null;
            break;

            case SelectorType.Holdable:
                currentGridObj = null;
                currentHoldableObj = (HoldableItem)_obj;
            break;

            case SelectorType.Blueprint:
                currentGridObj = (GridboundObject)_obj;
                currentHoldableObj = null;
            break;

            case SelectorType.Object:
                currentGridObj = (GridboundObject)_obj;
                currentHoldableObj = null;
            break;

            case SelectorType.Demolish:
                currentGridObj = null;
                currentHoldableObj = null;
            break;

            case SelectorType.Inquiry:
                currentGridObj = null;
                currentHoldableObj = null;
            break;

            case SelectorType.Boxing:
                currentGridObj = null;
                currentHoldableObj = null;
            break;
        }
        
        currentSelectorType = _type;
        UpdateSelectorModel();
    }

    public void RotateSelector() {
        currentSelectorDir = GetNextDir(currentSelectorDir);
    }

    public void CounterRotateSelector() {
        currentSelectorDir = GetLastDir(currentSelectorDir);
    }

    public Quaternion GetSelectorRotation()
    {
        return Quaternion.Euler(0, GetRotationAngle(currentSelectorDir), 0);
    }

    public GridboundObject GetCurrentSelector()
    {
        return currentGridObj;
    }

    public enum Dir {
        Up, Right, Down, Left
    }

    public static Dir GetNextDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
        }
    }

    public static Dir GetLastDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Up: return Dir.Left;
            case Dir.Right: return Dir.Up;
            case Dir.Down: return Dir.Right;
            case Dir.Left: return Dir.Down;
        }        
    }   

    public static int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }
}
