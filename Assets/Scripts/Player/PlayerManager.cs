using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Scripts.Utils;

public class PlayerManager : MonoBehaviour
{
    // Systems
    public PlayerMovementSystem movement { get; private set; }
    public PlayerHandsSystem hands { get; private set; }
    public PlayerBuildingSystem building { get; private set; }
    public PlayerCraftingSystem crafting { get; private set; }
    public PlayerMoneySystem money { get; private set; }
    public UIManager ui { get; private set; }
    public PlayerAnimationManager anim { get; private set; }
    
    // Inputs
    public MouseManager mouse { get; private set; }
    public KeyboardManager kb { get; private set; }

    // Body References
    public Rigidbody rb { get; private set; }
    public CapsuleCollider bodyCollider { get; private set; }
    public GhostController ghostController { get; private set; }
    public HandHold leftHand;
    public HandHold rightHand;
    public CarryPoint carry { get; private set; }
    public InteractZone interact { get; private set; }
    public LineRenderer dragLine { get; private set; }

    // Camera
    public Camera mainCam { get; private set; }
    public CinemachineVirtualCamera vCam { get; private set; }
    public CinemachineOrbitalTransposer vCamTransposer { get; private set; }


    private void Awake()
    {
        movement = GetComponent<PlayerMovementSystem>();
        building = GetComponent<PlayerBuildingSystem>();
        crafting = GetComponent<PlayerCraftingSystem>();
        hands = GetComponent<PlayerHandsSystem>();
        money = GetComponent<PlayerMoneySystem>();
        mouse = GetComponent<MouseManager>();
        kb = GetComponent<KeyboardManager>();
        ui = GetComponentInChildren<UIManager>();
        anim = GetComponent<PlayerAnimationManager>();
        rb = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();
        ghostController = GetComponentInChildren<GhostController>();
        carry = GetComponentInChildren<CarryPoint>();
        interact = GetComponentInChildren<InteractZone>();
        dragLine = GetComponent<LineRenderer>();
        mainCam = GameManager.Master.mainCam;
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        vCamTransposer = GetComponentInChildren<CinemachineOrbitalTransposer>();
    }
}
