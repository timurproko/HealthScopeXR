using UnityEngine;

public class ShaderClipSetter : MonoBehaviour
{
    [SerializeField] private GameObject _panelInteractable;

    [SerializeField, Range(0, 1)] private int _invertX;
    [SerializeField, Range(0, 1)] private int _invertY;
    [SerializeField, Range(0, 1)] private int _invertZ;

    [SerializeField, Range(0, 1)] private float _clipX = 1f;
    [SerializeField, Range(0, 1)] private float _clipY = 1f;
    [SerializeField, Range(0, 1)] private float _clipZ = 1f;

    private static readonly int ClipY = Shader.PropertyToID("_ClipY");
    private static readonly int ClipX = Shader.PropertyToID("_ClipX");
    private static readonly int ClipZ = Shader.PropertyToID("_ClipZ");

    private static readonly int InvertX = Shader.PropertyToID("_InvertX");
    private static readonly int InvertY = Shader.PropertyToID("_InvertY");
    private static readonly int InvertZ = Shader.PropertyToID("_InvertZ");

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        var clipXI = _panelInteractable.transform.localScale.y;

        if (_renderer != null && _renderer.sharedMaterial != null)
        {
            _renderer.sharedMaterial.SetFloat(ClipX, 1 - clipXI);
            _renderer.sharedMaterial.SetFloat(ClipY, _clipY);
            _renderer.sharedMaterial.SetFloat(ClipZ, _clipZ);
            _renderer.sharedMaterial.SetFloat(InvertX, _invertX);
            _renderer.sharedMaterial.SetFloat(InvertY, _invertY);
            _renderer.sharedMaterial.SetFloat(InvertZ, _invertZ);
        }
    }
}