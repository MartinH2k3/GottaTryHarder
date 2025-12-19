using Managers;
using UnityEngine;
using Utils;

namespace Other
{
public class HintUnlocker: MonoBehaviour
{
    [SerializeField] private string[] hintIds;
    [SerializeField] private HintUnlockerActionType actionType;
    [SerializeField] protected LayerMask playerLayer;

    private void Awake() {
        if (playerLayer == 0) {
            int playerLayerIndex = LayerMask.NameToLayer("Player");
            if (playerLayerIndex != -1) {
                playerLayer = 1 << playerLayerIndex;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!Helpers.LayerInLayerMask(other.gameObject.layer, playerLayer)) return;


        switch (actionType) {
            case HintUnlockerActionType.Lock:
                foreach (var hintID in hintIds)
                    HintManager.Instance.LockHint(hintID);
                break;
            case HintUnlockerActionType.Unlock:
                foreach (var hintID in hintIds)
                    HintManager.Instance.UnlockHint(hintID);
                break;
            default:
                break;
        }

        Destroy(this);
    }
}

public enum HintUnlockerActionType
{
    Unlock,
    Lock
}
}