using UnityEngine;

namespace Other
{

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxMultiplier = 0.5f;

    private Vector3 _lastCameraPosition;

    private void Start()
    {
        _lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - _lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxMultiplier, 0f, 0f);
        _lastCameraPosition = cameraTransform.position;
    }
}

}