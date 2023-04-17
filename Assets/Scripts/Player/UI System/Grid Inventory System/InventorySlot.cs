using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    // Settings
    [SerializeField] private Gradient tempGradient;
    [SerializeField] private Gradient qualityGradient;
    [SerializeField] private Sprite defaultImage;

    // States
    private HoldableObject obj;

    // Cache
    private PlayerManager player;
    private Image image;
    private TextMeshProUGUI tempText;
    private TextMeshProUGUI qualityText;

    private void Awake() {
        player = transform.root.GetComponent<PlayerManager>();
        image = transform.Find("Sprite").GetComponent<Image>();
        tempText = transform.Find("Temperature").GetComponent<TextMeshProUGUI>();
        qualityText = transform.Find("Quality").GetComponent<TextMeshProUGUI>();
    }

    private void UpdateTextColors(FoodObject _obj) {
        tempText.color = tempGradient.Evaluate(Mathf.Clamp(_obj.temperature / 100, 0, 1));
        qualityText.color = qualityGradient.Evaluate(Mathf.Clamp(_obj.quality / 100, 0, 1));
    }

    private void UpdateText(object _obj) {
        switch (_obj) {
            case FoodObject food:
                tempText.text = Mathf.RoundToInt(food.temperature).ToString()+ "°";
                qualityText.text = Mathf.RoundToInt(food.quality).ToString();
                UpdateTextColors(food);
            break;

            case DishObject dish:
            case HoldableObject holdable:
            default:
                tempText.text = "";
                qualityText.text = "";
            break;
        }
    }

    public void UpdateObject(object _obj) {
        obj = (HoldableObject)_obj;
        image.sprite = obj.image != null ? obj.image : defaultImage;        
        UpdateText(_obj);
    }

    public void OnMouseEnter() {
        player.ui.inventory.SetMouseoverInventorySlot(this.transform.GetSiblingIndex());
        player.mouse.SwitchToPointer();
    }

    public void OnMouseExit() {
        player.ui.inventory.SetMouseoverInventorySlot(null);
        player.mouse.SwitchToArrow();
    }
}