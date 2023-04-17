using System.Collections;
using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // [SerializeField] private HeatMapVisual heatMapVisual;
    // [SerializeField] private HeatMapBoolVisual heatMapBoolVisual;
    [SerializeField] private HeatMapGenericVisual heatMapGenericVisual;
    private GridSector<HeatMapGridObject> heatGrid;
    // private GridSector<StringGridObject> stringGrid;

    int gridWidth = 50;
    int gridHeight = 50;

    private Vector3 CalculateOffset() {
        return new Vector3(-(gridWidth / 2 * GameManager.Master.grid.cellSize), 0, -(gridHeight / 2 * GameManager.Master.grid.cellSize));
    }

    private void Start()
    {
        // stringGrid = new GridSector<StringGridObject>(gridWidth, gridHeight, cellSize, CalculateOffset(), (GridSector<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));
        // heatGrid = new GridSector<HeatMapGridObject>(gridWidth, gridHeight, CalculateOffset(), (GridSector<HeatMapGridObject> g, int x, int y) => new HeatMapGridObject(g, x, y));

        // heatMapGenericVisual.SetGrid(heatGrid);
    }

    private void Update()
    {
        // Vector3 position = GetMousePosition();

        // if (Input.GetMouseButtonDown(0))
        // {
        //     HeatMapGridObject gridObject = grid.GetGridObject(position);
        //     if (gridObject != null) {
        //         gridObject.AddValue(5);
        //     }
        // }

        // if (Input.GetMouseButtonDown(1))
        // {
        //     Debug.Log(grid.GetGridObject(GetMousePosition()));
        // }

        // if (Input.GetKeyDown(KeyCode.A)) {
        //     stringGrid.GetGridObject(position).AddLetter("A");
        // }

        // if (Input.GetKeyDown(KeyCode.B)) {
        //     stringGrid.GetGridObject(position).AddLetter("B");
        // }

        // if (Input.GetKeyDown(KeyCode.C)) {
        //     stringGrid.GetGridObject(position).AddLetter("C");
        // }


        // if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //     stringGrid.GetGridObject(position).AddNumber("1");
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha2)) {
        //     stringGrid.GetGridObject(position).AddNumber("2");
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha3)) {
        //     stringGrid.GetGridObject(position).AddNumber("3");
        // }
    }

    // private Vector3 GetMousePosition()
    // {
    //     return Camera
    //         .main
    //         .ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 75));
    // }
}

public class HeatMapGridObject {
    private const int MIN = 0;
    private const int MAX = 100;

    public int value;
    private GridSector<HeatMapGridObject> grid;
    private int x;
    private int y;

    public HeatMapGridObject(GridSector<HeatMapGridObject> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }
    
    public void AddValue(int addValue) {
        value += addValue;
        value = Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGridObjectChanged(x, y);
    }

    public float GetValueNormalized() {
        return (float)value / MAX;
    }

    public override string ToString() {
        return value.ToString();
    }
}

// public class StringGridObject {
//     private string letters;
//     private string numbers;

//     private GridSector<StringGridObject> grid;
//     private int x;
//     private int y;

    // public StringGridObject(GridSector<StringGridObject> grid, int x, int y) {
    //     this.grid = grid;
    //     this.x = x;
    //     this.y = y;
    //     letters = "";
    //     numbers = "";
    // }

    // public void AddLetter(string letter) {
    //     letters += letter;
    //     grid.TriggerGridObjectChanged(x, y);
    // }

    // public void AddNumber(string number) {
    //     numbers += number;
    //     grid.TriggerGridObjectChanged(x, y);
    // }

    // public override string ToString() {
    //     return letters + "\n" + numbers;
    // }
// }