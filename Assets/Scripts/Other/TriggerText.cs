using UnityEngine;
using Utils;

namespace Other
{
public class TriggerText: Hint
{
    [SerializeField] private GameObject textObject;

    protected override void Awake() {
        base.Awake();

        // handle possible missing references
        if (textObject == null) {
            var tmp = GetComponentInChildren<TMPro.TextMeshPro>();
            if (tmp != null) {
                textObject = tmp.gameObject;
            }
        }

        // Initialize text object to be inactive
        if (textObject != null)
            textObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!Helpers.LayerInLayerMask(other.gameObject.layer, playerLayer))
            return;

        ShowHint();
    }

    protected override void ShowHint() {
        base.ShowHint();
        if (textObject != null)
            textObject.SetActive(true);
    }

    /// <summary> Toggles the visibility of the text object. Used by outside scripts. </summary>
    public void ToggleText() {
        if (textObject == null)
            return;

        if (!textObject.activeSelf)
            ShowHint();
        else
            textObject.SetActive(false);
    }


}
}