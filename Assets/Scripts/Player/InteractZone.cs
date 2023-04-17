using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractZone : MonoBehaviour
{
    public LayerMask interactableLayers;

    public void Interact() {
        foreach (Transform _tgt in GetInteractables()) {
            Debug.Log(_tgt.name);
        }
    }

    public List<Transform> GetInteractables() {
        Collider[] interactables = Physics.OverlapSphere(transform.position + new Vector3(0, transform.localPosition.z - transform.localPosition.y, 0.1f), transform.localPosition.z, interactableLayers);
        List<Transform> targets = new List<Transform>();
        if (interactables.Length > 0)
            foreach (Collider tgt in interactables) {
                targets.Add(tgt.transform);
            }
        return targets;
    }

    public bool CanInteract(Transform _target) {
        return GetInteractables().Contains(_target);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, transform.localPosition.z - transform.localPosition.y, 0.1f), transform.localPosition.z);
    }
}
