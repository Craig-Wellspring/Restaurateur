using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    [SerializeField] private GameObject result;
    [SerializeField] private int resultQuantity = 1;
    [SerializeField] private bool alternateResultRotation = true;
    [SerializeField] private GameObject byproduct;
    [SerializeField] private int byproductQuantity = 0;
    [SerializeField] private float requiredWork = 100f;
    public float cutProgress { get; private set; } = 0f;
    private bool cuttable = true;
    [SerializeField] private Vector3 modCheckArea;

    private SliderController bar;

    private void Awake() {
        bar = GetComponentInChildren<SliderController>();

        if (cutProgress > 0)
            bar.SetShow(true);
    }
    public bool Cut(float _work) {
        if (cuttable) {
            if (!bar.doShow && cutProgress == 0) {
                bar.SetShow(true);
                bar.Show();
                bar.SetMaxValue(requiredWork);
            }

            cutProgress += _work * Time.deltaTime;
            bar.SetValue(cutProgress);

            if (cutProgress >= requiredWork)
                CompleteCut();
            
            return true;
        }
        return false;
    }

    public void PreventCutting() {
        cuttable = false;
    }

    public void SetCutProgress(float _new) {
        cutProgress = _new;
    }

    private void CompleteCut() {
        int placementIndex = 0;

        bool upsideDown = Physics.Raycast(transform.position, transform.up, 2f, LayerMask.GetMask("Ground"));
        Vector3 GetOffset(int _index) {
            return ((upsideDown ? -transform.up : transform.up) * 0.02f * _index);
        } 

        // Spawn result objects
        for (int i = 1; i < resultQuantity + 1; i++) {
            Transform resultItem = Instantiate(result.transform, transform.position, transform.rotation);
            resultItem.name = result.name;
            if (TryGetComponent<FoodItem>(out FoodItem _thisFood) && resultItem.TryGetComponent<FoodItem>(out FoodItem _newFood)) {
                _newFood.Overwrite(_thisFood);
            }

            // resultObj.Rotate;
            resultItem.position = resultItem.position + GetOffset(placementIndex);
            placementIndex++;

            if (alternateResultRotation) {
                if (upsideDown ? (i % 2 != 0) : (i % 2 == 0)) {
                    resultItem.Rotate(180,0,0);
                }
            }

            // Spawn byproduct object
            if (byproduct && byproductQuantity > 0) {
                Transform byproductObj = Instantiate(byproduct.transform, transform.position, transform.rotation);
                byproductObj.name = byproduct.name;
                byproductObj.position = byproductObj.position + GetOffset(placementIndex);
                byproductQuantity--;
                placementIndex++;
            }

        }

        // Destroy self
        Destroy(gameObject);
    }
}
