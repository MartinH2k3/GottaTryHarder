using UnityEngine;
using Utils;

namespace Other
{
public class SecretItemToggle: MonoBehaviour
{
    [SerializeField] private GameObject secretItem;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] private SecretDoorActionType actionType = SecretDoorActionType.Show;

    private void Awake() {
        if (playerLayer == 0) {
            int playerLayerIndex = LayerMask.NameToLayer("Player");
            if (playerLayerIndex != -1) {
                playerLayer = 1 << playerLayerIndex;
            }
        }
    }

    private void HideSecretItem() {
        if (secretItem != null) {
            secretItem.SetActive(false);
        }
    }

    private void ShowSecretItem() {
        if (secretItem != null) {
            secretItem.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!Helpers.LayerInLayerMask(other.gameObject.layer, playerLayer))
            return;

        switch (actionType) {
            case SecretDoorActionType.Hide:
                HideSecretItem();
                break;
            case SecretDoorActionType.Show:
                ShowSecretItem();
                break;
            case SecretDoorActionType.Toggle:
                if (secretItem != null) {
                    secretItem.SetActive(!secretItem.activeSelf);
                }

                break;
            default:
                break;
        }
    }
}

public enum SecretDoorActionType
{
    Hide,
    Show,
    Toggle
}
}