using UnityEngine;
using UnityEngine.Serialization;

namespace SweetSugar.Scripts.Blocks
{
    public class BindLayer : MonoBehaviour
    {
        [FormerlySerializedAs("layer")] [Header("Binds the layer order in editor")]
        public int order;
    }
}