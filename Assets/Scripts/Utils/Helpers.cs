using UnityEngine;

namespace Utils
{
public static class Helpers
{
    public static bool LayerInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask & (1 << layer)) != 0;
    }
    public static float StunDurationEased(float baseDuration, float strength) {
        return Mathf.Log(1+strength) * baseDuration;
    }
}
}