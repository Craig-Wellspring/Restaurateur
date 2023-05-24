using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FoodItem : HoldableItem, IClickable
{
    public enum FoodType {
        None,
        Fish,
        Shellfish,
        Meat,
        Poultry,
        Grain,
        Nut,
        Vegetable,
        Fruit,
        HerbSpice,
        Dairy,
        Fat,
    }

    public enum DishComponentType {
        None,
        Main,
        Side,
        Topping,
        Sauce,
        Garnish
    }

    // Settings
    [SerializeField] private FoodType foodType;
    public DishComponentType componentType;
    public float mass = 1f;
    
    // Quality and Flavor
    public float quality { get; private set; } = 1f;
    public List<Seasoning.Flavor> flavorProfile = new List<Seasoning.Flavor>() {
        new Seasoning.Flavor(Seasoning.FlavorType.Sweet, 0),
        new Seasoning.Flavor(Seasoning.FlavorType.Sour, 0),
        new Seasoning.Flavor(Seasoning.FlavorType.Spicy, 0),
        new Seasoning.Flavor(Seasoning.FlavorType.Salty, 0),
        new Seasoning.Flavor(Seasoning.FlavorType.Savory, 0),
        new Seasoning.Flavor(Seasoning.FlavorType.Pungent, 0),
        new Seasoning.Flavor(Seasoning.FlavorType.Bitter, 0),
        new Seasoning.Flavor(Seasoning.FlavorType.Menth, 0),
    };

    // Temperature
    [Tooltip("Density of material. Higher conductivity changes temperature more quickly. 1 is equally as dense as water. 0 is a perfect thermal insulator.")]
    public float thermalConductivity = 1f;
    public float foodTemp { get; private set; } = 70f;
    public float environsTemp { get; private set; } = 70f;

    // Cache
    public FoodUIManager ui { get; private set; }

    private void Awake() {
        TimeManager.OnTick += StabilizeTemp;
        base.LoadRefs();

        ui = GetComponentInChildren<FoodUIManager>();

        // Find mass
        TryGetComponent<Rigidbody>(out Rigidbody _rb);
        if (_rb.mass != mass) _rb.mass = mass;

        // Populate flavor profile
        // foreach (string _flavor in System.Enum.GetNames(typeof(Seasoning.FlavorType))) {
        //     Seasoning.FlavorType _type = (Seasoning.FlavorType)System.Enum.Parse(typeof(Seasoning.FlavorType), _flavor);
        //     flavorProfile.Add(new Seasoning.Flavor(_type, 0));
        // }
    }

    private void OnDestroy() {
        TimeManager.OnTick -= StabilizeTemp;
    }

    // Season
    public override void Click(PlayerManager _player, int _L0orR1) {
        base.Click(_player, _L0orR1);

        // Check hand
        HandHold _hand = _L0orR1 == 0 ? _player.leftHand : _player.rightHand;
        Transform handObj = _hand.GetHeldObject();

        // If this object is clicked and the player is holding an object
        if (handObj != null
            // and the clicked object is not the held object
            && handObj != this.transform
            // and the clicked object is not a seasoning
            && !GetComponent<Seasoning>()
            // and the held object is a seasoning
            && handObj.TryGetComponent<Seasoning>(out Seasoning _seasoning)) {
            // Season the clicked object with the held seasoning
            if (_seasoning.Consume()) {
                AddFlavor(_seasoning.type, _seasoning.intensity);
            }
        }
    }

    public void Overwrite(FoodItem _source) {
        if(TryGetComponent<Perishable>(out Perishable _perishable) && _source.TryGetComponent<Perishable>(out Perishable _sourcePerishable)) {
            _perishable.SetContamination((float)_sourcePerishable.contamination);
        }

        SetQuality(_source.quality);
        OverrideFlavorProfile(_source.flavorProfile);
        SetTemp(_source.foodTemp);
    }

    public override HoldableObject GenerateObject() {
        TryGetComponent<Perishable>(out Perishable _perishable);
        TryGetComponent<Cookable>(out Cookable _cookable);
        TryGetComponent<Cuttable>(out Cuttable _cuttable);
        GameObject _go = prefab == null ? this.gameObject : prefab;

        return FoodObject.Create(_go, sprite, quality, flavorProfile, mass, thermalConductivity, foodTemp, _perishable?.contamination, _cookable?.cookedProgress + _cookable?.overcookedProgress, _cuttable?.cutProgress);
    }

    public override HoldableObject GenerateObject(Vector2? _modifier) {
        TryGetComponent<Perishable>(out Perishable _perishable);
        TryGetComponent<Cookable>(out Cookable _cookable);
        TryGetComponent<Cuttable>(out Cuttable _cuttable);
        GameObject _go = prefab == null ? this.gameObject : prefab;
        float _quality = (_modifier != null && _modifier != Vector2.zero) ? Random.Range(_modifier.Value.x, _modifier.Value.y) : quality;

        return FoodObject.Create(_go, sprite, _quality, flavorProfile, mass, thermalConductivity, foodTemp, _perishable?.contamination, _cookable?.cookedProgress + _cookable?.overcookedProgress, _cuttable?.cutProgress);
    }

    // Quality
    private void AssessQuality() {
        string _assessment = "";
        _assessment += "Flavor profile for " + transform.name + ": (";
        foreach(var _flavor in flavorProfile) {
            _assessment += _flavor.type + ": " + _flavor.intensity + ", ";
        }

        if (TryGetComponent<Cookable>(out Cookable _cook)) {
            _assessment += "Cook Quality: " + (_cook.cookedProgress - _cook.overcookedProgress);
        }
        Debug.Log(_assessment);
    }

    public float IncreaseQuality(float _added) {
        quality += _added;
        return quality;
    }

    public float DecreaseQuality(float _added) {
        quality -= _added;
        return quality;
    }

    public float SetQuality(float _new) {
        quality = _new;
        return quality;
    }
    

    // Temperature
    private void StabilizeTemp(object _sender, System.EventArgs _args) {
        foodTemp += CookingSystem.CalculateHeatChange(mass, thermalConductivity, foodTemp, environsTemp);
        UpdateTempDisplay(foodTemp);
        ui.temperatureGauge.TriggerSlider();
    }

    public void SetTemp(float _newTemp) {
        foodTemp = _newTemp;
        UpdateTempDisplay(foodTemp);
    }
    
    public void SetEnvironsTemp(float _newTemp) {
        environsTemp = _newTemp;
    }

    public void UpdateTempDisplay(float _temp) {
        ui.temperatureGauge.SetFillColor(ui.temperatureGauge.fillGradient.Evaluate(Mathf.Clamp(Scripts.Utils.UtilsClass.ConvertFToC(_temp) / 100, 0, 1)));
        ui.tempNumDisplay.text = Mathf.RoundToInt(_temp).ToString()+ "°";
    }

    // Flavor
    public void AddFlavor(Seasoning.FlavorType _flavor, int _amount = 1) {
        flavorProfile.Find(flv => flv.type == _flavor).AddFlavor(_amount);
        AssessQuality();
    }

    public void OverrideFlavorProfile(List<Seasoning.Flavor> _newFlavorProfile) {
        flavorProfile = _newFlavorProfile;
    }
}

// Scriptable Object
[CreateAssetMenu(menuName = "ScriptableObjects/Holdables/Food Object")]
public class FoodObject : HoldableObject
{
    public float quality;
    public List<Seasoning.Flavor> flavorProfile;
    public float mass;
    public float conductivity;
    public float temperature;
    public float? contamination;
    public float? doneness;
    public float? cutProgress;

    public static FoodObject Create(GameObject _prefab, Sprite _image, float _quality, List<Seasoning.Flavor> _flavorProfile, float _mass, float _conductivity, float _temperature, float? _contamination = null, float? _doneness = null, float? _cutProgress = null) {
        FoodObject _newObj = CreateInstance<FoodObject>();

        _newObj.prefab = _prefab;
        _newObj.image = _image;

        _newObj.quality = _quality;
        _newObj.flavorProfile = _flavorProfile;
        _newObj.mass = _mass;
        _newObj.conductivity = _conductivity;
        _newObj.temperature = _temperature;

        _newObj.contamination = _contamination;
        _newObj.doneness = _doneness;
        _newObj.cutProgress = _cutProgress;

        return _newObj;
    }

    public override Transform GenerateItemFromObj(Vector3 _pos, Quaternion _rot) {
        Transform _newObj = Instantiate(this.prefab.transform, _pos, _rot);
        _newObj.name = this.prefab.name;
        _newObj.TryGetComponent<FoodItem>(out FoodItem _newFood);
        _newFood.SetQuality(this.quality);
        _newFood.OverrideFlavorProfile(this.flavorProfile);
        _newFood.SetTemp(this.temperature);

        if (this.contamination != null && _newObj.TryGetComponent<Perishable>(out Perishable _newPerishable)) {
            _newPerishable.SetContamination((float)this.contamination);
        }
        if (this.doneness != null && _newObj.TryGetComponent<Cookable>(out Cookable _newCookable)) {
            _newCookable.SetCookProgress((float)this.doneness);
        }
        if (this.cutProgress != null && _newObj.TryGetComponent<Cuttable>(out Cuttable _newCuttable)) {
            _newCuttable.SetCutProgress((float)this.cutProgress);
        }

        return _newObj;
    }
}