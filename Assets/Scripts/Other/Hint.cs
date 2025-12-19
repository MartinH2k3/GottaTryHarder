using UnityEngine;

namespace Other
{
public class Hint: MonoBehaviour {
    [SerializeField] protected LayerMask playerLayer;

    public System.Action HintDisplayed;

    [SerializeField] private string hintId;
    public string HintID => string.IsNullOrEmpty(hintId) ? gameObject.name : hintId;

    protected virtual void Awake() {
        if (playerLayer == 0) {
            int playerLayerIndex = LayerMask.NameToLayer("Player");
            if (playerLayerIndex != -1) {
                playerLayer = 1 << playerLayerIndex;
            }
        }

    }

    protected virtual void ShowHint() {
        HintDisplayed?.Invoke();
    }

    /// <summary> If the hint was shown previously, removes it from the scene. </summary>
    public void RemoveHint() {
        gameObject.SetActive(false);
    }
}
}