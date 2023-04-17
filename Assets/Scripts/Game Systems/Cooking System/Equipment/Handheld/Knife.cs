using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour, IUtensil
{
    [Tooltip("How quickly a cut process progresses")]
    public float sharpness = 20f;

    public float minSharpness = 2f;
    public float maxSharpness = 50f;

    [Tooltip("Higher numbers will decrease in sharpness slower")]
    public float hardness = 5f;

    public bool Use(Transform _target) {
        if (_target.TryGetComponent<Cuttable>(out Cuttable _cut)) {
            if (_cut.Cut(sharpness)) {
                Dull(Time.deltaTime / hardness);
                return true;
            }
        }
        return false;
    }

    public void Sharpen(float _addSharpness) {
        if (sharpness < maxSharpness)
            sharpness += _addSharpness;
    }

    public void Dull(float _removeSharpness) {
        if (sharpness > minSharpness)
            sharpness -= _removeSharpness;
    }
}
