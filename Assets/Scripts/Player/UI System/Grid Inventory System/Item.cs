using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Item : MonoBehaviour {
    public Sprite sprite;
    public ItemType type;

    public GameObject prefab { get; private set; }
    [SerializeField] private AssetReferenceT<GameObject> prefabReference;

    private void Awake() {
        LoadRefs();
    }

    public void LoadRefs() {
        prefabReference.LoadAssetAsync<GameObject>().Completed += (_go) => { prefab = _go.Result; };
    }
}

public enum ItemType {
    None,
    Food,
    Equipment, // Gridbound items: Coolers, cookers, counters
    Gear, // Boxes, tools, containers, bags, wearables
    Cookware, // Kitchen items: Pots, pans, knives, whisks
    Tableware, // Dining items: Plates, cups, forks
    Rubbish, // Has no use
}