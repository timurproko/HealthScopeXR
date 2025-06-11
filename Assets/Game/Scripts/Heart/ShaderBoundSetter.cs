using UnityEngine;

[ExecuteAlways]
public class ShaderBoundSetter : MonoBehaviour
{
    private static readonly int WorldScaleID = Shader.PropertyToID("_WorldScale");
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        if (_renderer != null && _renderer.sharedMaterial != null)
        {
            Vector3 lossy = transform.lossyScale;
            _renderer.sharedMaterial.SetVector(WorldScaleID, lossy);
        }
    }
}