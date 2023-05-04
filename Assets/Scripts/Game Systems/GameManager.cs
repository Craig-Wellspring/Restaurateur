using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Master;

    // Systems
    public TimeManager time { get; private set; }
    
    public Camera mainCam { get; private set; }
    private SpawnPlayer spawner;
    public GridManager grid { get; private set; }
    public Transform deliveryLocation { get; private set; }

    // States
    public float airTemp { get; private set; } = 70f;

    // Players
    public List<PlayerManager> players { get; private set; }

    private void Awake() {
        Master = this;

        players = new List<PlayerManager>();
        mainCam = Camera.main;
        spawner = GetComponentInChildren<SpawnPlayer>();
        time = GetComponent<TimeManager>();
        grid = GetComponent<GridManager>();
        deliveryLocation = transform.Find("Delivery Location");

        time.LoadTime(new TimeManager.TimeObject(6, 58, 24, 7, 4, 5, 2, 8));
    }

    private void Start() {
        players.Add(spawner.Spawn().GetComponent<PlayerManager>());
    }
}
