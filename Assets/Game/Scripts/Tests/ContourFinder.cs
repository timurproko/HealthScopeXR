// using UnityEngine;
// using OpenCvSharp;
// using OpenCvSharp.Demo;
// using Sirenix.OdinInspector;
//
// namespace Game
// {
//     public class ContourFinder : WebCamera
//     {
//         [Header("Contour Finder")] [SerializeField]
//         private int _cameraIndex;
//
//         [SerializeField] private FlipMode _cameraFlipMode;
//         [SerializeField] private PolygonCollider2D _polygonCollider;
//
//         [Header("Image Processing")] [SerializeField]
//         private bool _showProcessingImage;
//
//         [ShowIf("@!_showProcessingImage")] [SerializeField]
//         private Color _color = Color.red;
//
//         [ShowIf("@!_showProcessingImage")] [SerializeField, Range(0, 10)]
//         private int _thickness = 2;
//
//         // [SerializeField, Range(0, 255)] private float _threshold = 128;
//         [SerializeField, Range(0, 255)] private int _minThreshold = 100;
//         [SerializeField, Range(0, 255)] private int _maxThreshold = 200;
//
//         [SerializeField, Range(0, 100)] private float _curveAccuracy = 1f;
//         [SerializeField, Range(0, 5000)] private float _minArea = 1000f;
//
//         private Mat _inputImage;
//         private readonly Mat _processImage = new();
//         private Point[][] _contour;
//         private HierarchyIndex[] _hierarchyIndex;
//         private Vector2[] _vectorList;
//
//         protected override void Awake()
//         {
//             var devices = WebCamTexture.devices;
//             if (devices.Length > 0 && _cameraIndex >= 0 && _cameraIndex < devices.Length)
//             {
//                 DeviceName = devices[_cameraIndex].name;
//             }
//             else
//             {
//                 Debug.LogWarning($"Invalid camera index {_cameraIndex}. Using default.");
//                 base.Awake();
//             }
//         }
//
//         protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
//         {
//             _inputImage = OpenCvSharp.Unity.TextureToMat(input);
//             Cv2.Flip(_inputImage, _inputImage, _cameraFlipMode);
//
//             Cv2.CvtColor(_inputImage, _processImage, ColorConversionCodes.BGR2GRAY);
//             // Cv2.Threshold(_processImage, _processImage, _threshold, 255, ThresholdTypes.BinaryInv);
//             Cv2.InRange(_processImage, new Scalar(_minThreshold), new Scalar(_maxThreshold), _processImage);
//             Cv2.FindContours(_processImage, out _contour, out _hierarchyIndex, RetrievalModes.Tree,
//                 ContourApproximationModes.ApproxSimple);
//
//             _polygonCollider.pathCount = 0;
//
//             foreach (var contour in _contour)
//             {
//                 Point[] points = Cv2.ApproxPolyDP(contour, _curveAccuracy, true);
//                 var area = Cv2.ContourArea(contour);
//                 if (area > _minArea)
//                 {
//                     var color = new Scalar(_color.b * 255, _color.g * 255, _color.r * 255);
//                     DrawContour(_inputImage, color, _thickness, points);
//                     _polygonCollider.pathCount++;
//                     _polygonCollider.SetPath(_polygonCollider.pathCount - 1, PointsToVector2(points));
//                 }
//             }
//
//             if (!output)
//             {
//                 output = OpenCvSharp.Unity.MatToTexture(_showProcessingImage ? _processImage : _inputImage);
//             }
//             else
//             {
//                 OpenCvSharp.Unity.MatToTexture(_showProcessingImage ? _processImage : _inputImage, output);
//             }
//
//             return true;
//         }
//
//         private Vector2[] PointsToVector2(Point[] points)
//         {
//             _vectorList = new Vector2[points.Length];
//             for (int i = 0; i < points.Length; i++)
//             {
//                 float x = points[i].X - _inputImage.Width / 2;
//                 float y = points[i].Y - _inputImage.Height / 2;
//
//                 y = 1f - y;
//
//                 _vectorList[i] = new Vector2(x, y);
//             }
//
//             return _vectorList;
//         }
//
//
//         private void DrawContour(Mat image, Scalar color, int thickness, Point[] points)
//         {
//             for (int i = 1; i < points.Length; i++)
//             {
//                 Cv2.Line(image, points[i - 1], points[i], color, thickness);
//             }
//
//             Cv2.Line(image, points[^1], points[0], color, thickness);
//         }
//     }
// }