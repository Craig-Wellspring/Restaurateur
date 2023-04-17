using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishItem : HoldableItem
{
    // Settings
    public bool disposable = false;
    public float grimeScale = 1;
    public float grimeIntensity = 4.5f;

    // States
    public bool clean { get; private set; } = true;

    // Cache
    private Material dishMaterial;

    private void Awake() {
        base.LoadRefs();
        dishMaterial = GetComponent<MeshRenderer>().material;
        dishMaterial.SetFloat("_TextureScale", grimeScale);
    }

    public void SetClean(bool _isClean) {
        clean = _isClean;
    }

    public void CleanDish() {
        SetClean(true);
        dishMaterial.SetFloat("_GrimeLevel", 0);
    }

    public void SoilDish() {
        SetClean(false);
        dishMaterial.SetFloat("_GrimeLevel", grimeIntensity);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public override HoldableObject GenerateObject() {
        GameObject _go = prefab == null ? this.gameObject : prefab;
        return DishObject.Create(_go, sprite, clean);
    }
}


// Scriptable Object
public class DishObject : HoldableObject
{
    bool clean;

    public static DishObject Create(GameObject _prefab, Sprite _image, bool _clean) {
        DishObject _newObj = CreateInstance<DishObject>();

        _newObj.prefab = _prefab;
        _newObj.image = _image;

        _newObj.clean = _clean;

        return _newObj;
    }

    public override Transform GenerateItemFromObj(Vector3 _pos, Quaternion _rot) {
        Transform _newObj = Instantiate(this.prefab.transform, _pos, _rot);
        _newObj.name = this.prefab.name;
        _newObj.TryGetComponent<DishItem>(out DishItem _newDish);
        _newDish.SetClean(this.clean);
        
        return _newObj;
    }
}