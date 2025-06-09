using UnityEngine;
using UnityEngine.UI;
using PassthroughCameraSamples;

namespace Game
{
    public class CamTexture : MonoBehaviour
    {
        [SerializeField] private WebCamTextureManager _webCamTextureManager;
        [SerializeField] private RawImage _image;
        [SerializeField] private Material _filterMaterial;

        private void Update()
        {
            var camTex = _webCamTextureManager.WebCamTexture;
        
            if (!camTex || !camTex.isPlaying || !camTex.didUpdateThisFrame)
                return;
            
            _image.texture = camTex;
            _image.material = _filterMaterial;
        }
    }
}