using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridboundItem : Item
{
    public GridboundObject gridboundObject { get; private set; }
    private Vector2Int origin;
    private GhostController.Dir dir;
    private GridSector<BuildingGridTile> homeGrid;

    public static GridboundItem Create(Vector3 _worldPosition, Vector2Int _origin, GhostController.Dir _dir, GridboundObject _objToPlace) {
        Transform placedObjectTransform = Instantiate(_objToPlace.prefab.transform, _worldPosition, Quaternion.Euler(0, GhostController.GetRotationAngle(_dir), 0));
        placedObjectTransform.name = _objToPlace.prefab.name;

        GridboundItem placedObject = placedObjectTransform.GetComponent<GridboundItem>();
        placedObject.gridboundObject = _objToPlace;
        placedObject.origin = _origin;
        placedObject.dir = _dir;
        placedObject.homeGrid = GameManager.Master.grid.GetGridFromPosition(_worldPosition);

        return placedObject;
    }

    public Vector3 GetObjectCenterWorldPos() {
        Vector2Int rotationOffset = gridboundObject.GetRotationOffset(dir);
        Vector3 localOffset = new Vector3((gridboundObject.gridWidth * GameManager.Master.grid.cellSize / 2) - rotationOffset.x / 2, 0, (gridboundObject.gridHeight * GameManager.Master.grid.cellSize / 2) - rotationOffset.y / 2);
        return this.transform.position + localOffset;
    }

    public List<Vector2Int> GetGridPositionList() {
        return gridboundObject.GetGridPositionList(origin, dir);
    }
    public void DestroySelf() {
        Destroy(gameObject);

        // Clear building grid of object
        foreach (Vector2Int gridPosition in GetGridPositionList())
            homeGrid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
    }
}
