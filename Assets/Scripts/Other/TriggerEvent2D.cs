using UnityEngine;
using UnityEngine.Events;

namespace Other
{
public class TriggerEvent2D : MonoBehaviour
{
    public UnityEvent onTriggerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke();
    }
}

}