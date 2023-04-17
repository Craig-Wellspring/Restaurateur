using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public void VisualizeGrid(GridSector<BuildingGridTile> _grid) {
        _grid.SetVisualizer(this.transform);
        MatchGridSize(_grid);
    }

    public void MatchGridSize(GridSector<BuildingGridTile> _grid) {
        GetComponent<SpriteRenderer>().size = new Vector3(_grid.GetWidth() * GameManager.Master.grid.cellSize, _grid.GetHeight() * GameManager.Master.grid.cellSize);
    }
}
