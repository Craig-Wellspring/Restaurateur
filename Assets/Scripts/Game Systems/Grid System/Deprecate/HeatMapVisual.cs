using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;

public class HeatMapVisual : MonoBehaviour
{
    private GridSector<int> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(GridSector<int> grid) {
        this.grid = grid;
        UpdateHeatMapVisual();

        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, GridSector<int>.OnGridValueChangedEventArgs e) {
        updateMesh = true;
    }

    private void LateUpdate() {
        if (updateMesh) {
            UpdateHeatMapVisual();
            updateMesh = true;
        }
    }

    private void UpdateHeatMapVisual() {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * GameManager.Master.grid.cellSize;
                
                int gridValue =  grid.GetGridObject(x, y);
                float gridValueNormalized = (float)gridValue / 100; //GridSector<int>.HEAT_MAP_MAX_VALUE;
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, gridValueUV, gridValueUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
