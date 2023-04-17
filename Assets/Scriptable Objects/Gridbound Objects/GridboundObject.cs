using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GridboundObject")]
public class GridboundObject : ScriptableObject
{
    public GameObject prefab;
    public int gridWidth;
    public int gridHeight;
    public float verticalMeters;
    
    public Vector2Int GetRotationOffset(GhostController.Dir dir) {
        switch (dir) {
            default:
            case GhostController.Dir.Down: return new Vector2Int(0, 0);
            case GhostController.Dir.Left: return new Vector2Int(0, gridWidth);
            case GhostController.Dir.Up: return new Vector2Int(gridWidth, gridHeight);
            case GhostController.Dir.Right: return new Vector2Int(gridHeight, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, GhostController.Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir) {
            default:
            case GhostController.Dir.Down:
            case GhostController.Dir.Up:
                for (int x = 0; x < gridWidth; x++) {
                    for (int y = 0; y < gridHeight; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case GhostController.Dir.Left:
            case GhostController.Dir.Right:
                for (int x = 0; x < gridHeight; x++) {
                    for (int y = 0; y < gridWidth; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
}
