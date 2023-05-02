using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardController : MonoBehaviour
{
    // Settings
    public float timeToHide = 3f;

    public enum StickyPosition {
        Top,
        Front
    }
    public StickyPosition stickyPosition;
    [SerializeField] private Transform billboardTransform;

    // Cache
    private CanvasGroup billboardCanvas;
    public bool isShowing { get; private set; } = false;
    private float timer = 0f;

    private void Awake() {
        billboardCanvas = billboardTransform.GetComponent<CanvasGroup>();
        if (!isShowing) {
            Hide();
        }
    }

    private void LateUpdate() {
        if (timer < timeToHide) {
            timer += Time.deltaTime;
            if (isShowing) {
                Vector3 offset = Vector3.zero;
                switch (stickyPosition) {
                    case StickyPosition.Top: offset = Vector3.up * transform.localPosition.y; break;
                    case StickyPosition.Front: offset = Vector3.up * transform.localPosition.z; break;
                }
                billboardTransform.position = transform.position + offset;
                transform.LookAt(billboardTransform.position + GameManager.Master.mainCam.transform.forward);
            }
        } else {
            if (isShowing) Hide();
        }
    }

    public void ResetTimer() {
        timer = 0f;
    }

    public void Show() {
        billboardCanvas.alpha = 1;
        isShowing = true;
        ResetTimer();
    }

    public void Hide() {
        billboardCanvas.alpha = 0;
        isShowing = false;
    }
}
