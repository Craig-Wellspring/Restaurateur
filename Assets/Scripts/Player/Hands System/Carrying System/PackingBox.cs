using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingBox : MonoBehaviour, IUsable
{
    public List<SpriteRenderer> labels;

    public bool isPacked { get; private set; } = false;
    [SerializeField] private GridboundObject packedObject;

    private void Start() {
        if (packedObject && !isPacked)
            Pack(packedObject.prefab.transform);
    }


    public void Use(PlayerManager _user) {
        if (isPacked) {
            // Switch to blueprint mode to place object
            if (!_user.building.blueprintModeOn)
                _user.building.ToggleBlueprintMode();
            _user.ghostController.ChangeSelector(GhostController.SelectorType.Object, packedObject);
        } else {
            // Try to pack object in front of player
            List<Transform> targets = _user.interact.GetInteractables();
            foreach (Transform tgt in targets) {
                if (tgt.TryGetComponent<Packable>(out Packable pck) && pck.CanPack()) {
                    Pack(tgt);
                    return;
                }
            }
        }
    }

    public void Pack(Transform _target) {
        _target.TryGetComponent<Packable>(out Packable _pk);
        packedObject = _pk?.packableObj;
        isPacked = true;

        packedObject.prefab.TryGetComponent<Item>(out Item _item);
        foreach (SpriteRenderer label in labels) {
            label.sprite = _item.sprite;
        }
    }

    public void Unpack() {
        packedObject = null;
        isPacked = false;

        foreach (SpriteRenderer label in labels) {
            label.sprite = null;
        }
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public GridboundObject GetPackedObject() {
        return packedObject;
    }
}
