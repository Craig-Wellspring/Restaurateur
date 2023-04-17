using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private PlayerManager player;
    public Animator animCtrlr { get; private set; }

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        animCtrlr = GetComponent<Animator>();        
    }

    private void Update()
    {
        animCtrlr.SetBool("IsGrounded", player.movement.isGrounded);
        animCtrlr.SetFloat("MoveSpeed", player.kb.movementInput.magnitude);
        animCtrlr.SetBool("IsCarrying", player.hands.isCarrying);        
    }

    public void TriggerLiftAnim() {
        player.hands.SetIsCarrying(true);
        player.movement.SetCanMove(false);
        animCtrlr.SetTrigger("OnLift");
    }

    public void LiftAnimConnectMoment() {
        animCtrlr.ResetTrigger("OnLift");
        player.movement.SetCanMove(true);
        player.carry.CarryObject(player.carry.carryTarget);

        // TODO: Start maintaining objectUp rotation
    }

    // TODO: LiftAnimEnd - Stop maintaining rotation

    public void TriggerDropAnim() {
        player.rb.isKinematic = true;

        animCtrlr.SetTrigger("OnDrop");
        player.movement.SetCanMove(false);

        // TODO: Start maintaining objectUp rotation
    }

    public void DropAnimDisconnectMoment() {
        player.rb.isKinematic = false;

        animCtrlr.ResetTrigger("OnDrop");
        player.movement.SetCanMove(true);
        
        player.carry.carryTarget.position = new Vector3(player.carry.carryTarget.position.x, player.transform.position.y, player.carry.carryTarget.position.z);
        player.carry.carryTarget.rotation = player.transform.rotation;
        player.carry.DropCarriedObject();   

        // TODO: Stop maintaining rotation
    }
}
