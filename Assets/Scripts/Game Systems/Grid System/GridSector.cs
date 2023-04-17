using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;

public class GridSector<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs: EventArgs {
        public int x;
        public int z;
    }
    public int gridID { get; private set; }

    private int width;

    private int height;

    private Vector3 originPosition;

    public Transform visualizer;
    public bool showVisualizer;

    private TGridObject[,] gridArray;

    public GridSector(int _id, Vector2Int _gridDimensions, Vector3 _originPosition, Func<GridSector<TGridObject>, int, int, TGridObject> _CreateGridObject)
    {
        this.gridID = _id;
        this.width = _gridDimensions.x;
        this.height = _gridDimensions.y;
        this.originPosition = _originPosition;
        this.visualizer = null;
        this.showVisualizer = false;

        gridArray = new TGridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = _CreateGridObject(this, x, z);
            }
        }

        // Debug lines
        bool showDebug = true;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    Debug
                        .DrawLine(GetWorldPosition(x, z),
                        GetWorldPosition(x, z + 1),
                        Color.white,
                        100f);
                    Debug
                        .DrawLine(GetWorldPosition(x, z),
                        GetWorldPosition(x + 1, z),
                        Color.white,
                        100f);
                }
            }

            Debug
                .DrawLine(GetWorldPosition(0, height),
                GetWorldPosition(width, height),
                Color.white,
                100f);
            Debug
                .DrawLine(GetWorldPosition(width, 0),
                GetWorldPosition(width, height),
                Color.white,
                100f);
            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) => {
                // debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
            };
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * GameManager.Master.grid.cellSize + originPosition;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public Bounds GetBounds() {
        return new Bounds(originPosition + (new Vector3(width, 0, height) / 2 * GameManager.Master.grid.cellSize), new Vector3(width * 0.333f, 50f, height * 0.333f));
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / GameManager.Master.grid.cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / GameManager.Master.grid.cellSize);
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetGridObject(x, z, value);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
        }
    }

    public void TriggerGridObjectChanged(int x, int z) {
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
    }

    public TGridObject GetGridObject(int x, int z) {
        if (x >= 0 && z >= 0 && x < width && z < height) {
            return gridArray[x, z];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public void SetVisualizer(Transform _visObj) {
        this.visualizer = _visObj;
    }

    public void ShowVisualizer(bool _show) {
        this.showVisualizer = _show;
        this.visualizer.gameObject.SetActive(_show);
    }
}


//// Grid Object Types

public class BuildingGridTile
{
    private GridSector<BuildingGridTile> objGrid;

    private int x;
    private int z;

    private GridboundItem placedGridObject;

    public BuildingGridTile(GridSector<BuildingGridTile> grid, int x, int z)
    {
        this.objGrid = grid;
        this.x = x;
        this.z = z;
    }

    public GridSector<BuildingGridTile> GetGrid() {
        return objGrid;
    }

    public void SetPlacedObject(GridboundItem _placedObject)
    {
        this.placedGridObject = _placedObject;
        objGrid.TriggerGridObjectChanged (x, z);
    }

    public GridboundItem GetPlacedObject()
    {
        return placedGridObject;
    }

    public void ClearPlacedObject()
    {
        placedGridObject = null;
        objGrid.TriggerGridObjectChanged (x, z);
    }

    public bool CanBuild()
    {
        return placedGridObject == null;
    }

    public override string ToString()
    {
        return "Grid Coordinates: (" + x + ", " + z + "), Grid ID: " + objGrid.gridID;
    }
}


public class FloorGridTile {
    private GridSector<FloorGridTile> grid;
    private Sprite image;
    private int x;
    private int z;

    public FloorGridTile(GridSector<FloorGridTile> grid, int _x, int _z, Sprite _image) {
        this.grid = grid;
        this.image = _image;
        
        this.x = _x;
        this.z = _z;
    }
}