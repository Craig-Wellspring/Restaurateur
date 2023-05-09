using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Global grid settings
    public float cellSize { get; private set; } = 0.25f;
    [SerializeField] private Transform gridVisualizerPrefab;

    // Building Grids
    public List<GridSector<BuildingGridTile>> buildingGrids = new List<GridSector<BuildingGridTile>>();


    private void Awake()
    {
        CreateBuildingGrid(new Vector2Int(100, 100), new Vector3(0,0,0));
    }

    public GridSector<BuildingGridTile> CreateBuildingGrid(Vector2Int _gridDimensions, Vector3 _originPos) {
        GridSector<BuildingGridTile> _newGrid = new GridSector<BuildingGridTile>(buildingGrids.Count + 1, _gridDimensions, _originPos - GridCenterOffset(_gridDimensions.x, _gridDimensions.y), (GridSector<BuildingGridTile> g, int x, int z) => new BuildingGridTile(g, x, z));
        buildingGrids.Add(_newGrid);

        Transform _visualizer = Instantiate(gridVisualizerPrefab, _originPos + new Vector3(0f, 0.003f, 0f), gridVisualizerPrefab.rotation);
        _visualizer.GetComponent<GridVisualizer>().VisualizeGrid(_newGrid);
        _visualizer.SetParent(this.transform);
        _visualizer.name = "Grid " + _newGrid.gridID + " Visualizer";
        _visualizer.gameObject.SetActive(false);

        return _newGrid;
    }

    private Vector3 GridCenterOffset(int _gridWidth, int _gridHeight)
    {
        return new Vector3(_gridWidth, 0, _gridHeight) / 2 * cellSize;
    }
    
    public GridSector<BuildingGridTile> GetGridFromPosition(Vector3 _pos) {
        foreach (GridSector<BuildingGridTile> _grid in buildingGrids) {
            if (_grid.GetBounds().Contains(_pos)) {
                return _grid;
            }
        }
        return null;
    }
}