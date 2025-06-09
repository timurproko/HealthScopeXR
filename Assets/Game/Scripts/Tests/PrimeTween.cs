using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class PrimeTweenTests : MonoBehaviour
    {
        [SerializeField] TweenSettings<float> _yPositionTweenSettings;

        [Button]
        private void GoUp()
        {
            Tween.PositionY(transform, _yPositionTweenSettings).OnComplete(SomeMethod);
        }

        private void SomeMethod()
        {
            Debug.Log("Tween is finished");
        }

        [Button]
        private void GoDown()
        {
            Tween.PositionY(transform, endValue: 0, duration: 1f, ease: Ease.Default);
        }

        [Button]
        private void ShakePosition()
        {
            Tween.ShakeLocalPosition(transform, strength: new Vector3(0, 1), duration: 1, frequency: 10);
        }

        [Button]
        private void Delay()
        {
            Tween.Delay(duration: 1f, SomeMethod);
        }
    }
}