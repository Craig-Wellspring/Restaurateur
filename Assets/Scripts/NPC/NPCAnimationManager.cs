using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationManager : MonoBehaviour
{
    private NPCManager npc;
    private Animator animator;

    private void Awake() {
        npc = GetComponent<NPCManager>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        animator.SetFloat("MoveSpeed", npc.nav.velocity.magnitude);
    }
}
