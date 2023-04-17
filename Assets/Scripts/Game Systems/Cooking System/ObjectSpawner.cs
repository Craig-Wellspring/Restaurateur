using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour, IClickable
{
    public GameObject objToSpawn;

    private void Awake() {
        if (objToSpawn != null) {
        GetComponent<MeshFilter>().sharedMesh = objToSpawn.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<MeshRenderer>().sharedMaterials = objToSpawn.GetComponent<MeshRenderer>().sharedMaterials;
        }
    }

    public void Click(PlayerManager _player, int _L0orR1) {
        if (!_player.hands.isCarrying) {
            // Check hands
            HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
            Transform handObj = _hand.GetHeldObject();
            // Hand empty - pick up item
            if (handObj == null) {
                if (objToSpawn != null) {
                    Transform collectedItem = Instantiate(objToSpawn.transform, transform.position, Quaternion.identity);
                    collectedItem.name = objToSpawn.name;
                    _hand.HoldObject(collectedItem);
                }
            }
        }
    }
}
