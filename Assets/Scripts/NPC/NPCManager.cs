using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCManager : MonoBehaviour
{
    public Collider body { get; private set; }
    public NPCAnimationManager anim { get; private set; }
    public NavMeshAgent nav { get; private set; }

    private void Awake() {
        body = GetComponent<Collider>();
        anim = GetComponent<NPCAnimationManager>();
        nav = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 _pos) {
        nav.SetDestination(_pos);
    }
}
