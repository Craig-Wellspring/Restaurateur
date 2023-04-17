using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerObject;

    public Transform Spawn() {
        Transform player = Instantiate(playerObject, transform.position, transform.rotation);
        player.gameObject.name = "Player Character";

        return player;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.3f);
    }
}
