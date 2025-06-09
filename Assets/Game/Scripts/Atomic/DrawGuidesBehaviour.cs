// using Atomic.Entities;
// using Prototype;
// using Shapes;
// using UnityEngine;
//
// public class DrawGuidesBehaviour : IEntityInit, IEntityShapes
// {
//     private IEntity entity;
//     private Color color = Color.yellow;
//
//     public void Init(in IEntity entity)
//     {
//         this.entity = entity;
//     }
//
//     public void OnShapesDraw(Camera cam, in IEntity entity)
//     {
//         cam.Draw(DrawSineWave);
//     }
//
//     private void DrawSineWave()
//     {
//         var center = entity.GetTransform().position;
//         float waveLength = 0.25f;
//         float amplitude = 0.025f;
//         float speed = 1f;
//         int segments = 50;
//
//         Vector3 start = center - new Vector3(waveLength / 2f, 0, 0);
//         Vector3 prev = start;
//
//         float time = Time.time * speed;
//
//         for (int i = 1; i <= segments; i++)
//         {
//             float t = (float)i / segments;
//             float x = t * waveLength;
//
//             float noiseInputX = t * 20f + time;
//             float y = (Mathf.PerlinNoise(noiseInputX, 0f) - 0.0f) * 2f * amplitude;
//
//             Vector3 current = start + new Vector3(x, y, 0);
//             Draw.Line(prev, current, 0.002f, color);
//             prev = current;
//         }
//     }
// }