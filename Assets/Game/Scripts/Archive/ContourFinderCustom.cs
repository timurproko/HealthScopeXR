// using UnityEngine;
// using OpenCvSharp;
// using PassthroughCameraSamples;
// using UnityEngine.UI;
//
// namespace Game
// {
//     public class ContourFinderCustom : MonoBehaviour
//     {
//         [SerializeField] private FlipMode _cameraFlipMode;
//         [SerializeField] private WebCamTextureManager m_webCamTextureManager;
//         [SerializeField] private RawImage m_image;
//
//         private WebCamTexture _cameraTexture;
//         private Texture2D _outputTexture;
//         private Mat _inputImage;
//
//         private void Update()
//         {
//             var camTex = m_webCamTextureManager.WebCamTexture;
//
//             if (camTex == null || !camTex.isPlaying || !camTex.didUpdateThisFrame)
//                 return;
//             
//             // ProcessTexture(camTex);
//             // m_image.texture = _outputTexture;
//             m_image.texture = camTex;
//         }
//
//         private void ProcessTexture(WebCamTexture input)
//         {
//             _inputImage = OpenCvSharp.Unity.TextureToMat(input);
//             Cv2.Flip(_inputImage, _inputImage, _cameraFlipMode);
//
//             if (_outputTexture == null)
//                 _outputTexture = new Texture2D(_inputImage.Width, _inputImage.Height, TextureFormat.RGBA32, false);
//
//             OpenCvSharp.Unity.MatToTexture(_inputImage, _outputTexture);
//         }
//     }
// }