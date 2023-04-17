using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardController : MonoBehaviour
{
    public enum StickyPosition {
        Top,
        Front
    }
    public StickyPosition stickyPosition;
    [SerializeField] private Transform togglePanel;
    public bool doShow { get; private set; } = false;

    private void LateUpdate() {
        if (togglePanel.gameObject.activeSelf) {
            Vector3 offset = Vector3.zero;
            switch (stickyPosition) {
                case StickyPosition.Top: offset = Vector3.up * transform.localPosition.y; break;
                case StickyPosition.Front: offset = Vector3.up * transform.localPosition.z; break;
            }
            togglePanel.position = transform.root.position + offset;
            transform.LookAt(transform.root.position + GameManager.Master.mainCam.transform.forward);
        }
    }

    public void Show() {
        togglePanel.gameObject.SetActive(true);
    }

    public void Hide() {
        togglePanel.gameObject.SetActive(false);
    }

    public void SetShow(bool _do) {
        doShow = _do;
    }
}
