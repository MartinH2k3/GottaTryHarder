using UnityEngine;

namespace Utils
{
public static class Helpers
{
    public static bool LayerInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask & (1 << layer)) != 0;
    }
}
}