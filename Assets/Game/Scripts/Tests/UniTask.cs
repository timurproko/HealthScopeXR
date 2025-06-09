using UnityEngine;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine.Networking;

namespace Game
{
    public class UniTaskTests : MonoBehaviour
    {
        [SerializeField] private Light _light;

        private void Start()
        {
            // MakePizza().Forget();
            // WaitForPlayer().Forget();
            // WaitForAnimation().Forget();
            // LightBlinkLoop().Forget();
            // SpawnAtInterval().Forget();
            Fetchdata().Forget();
        }

        private async UniTaskVoid Fetchdata()
        {
            string url;
            url = "https://www.google.com/asdasdasdasd";
            url = "https://www.google.com/";
            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();
            
            if(request.result == UnityWebRequest.Result.Success)
                Debug.Log(request.downloadHandler.text);
            else
                Debug.LogError(request.error);
        }

        private async UniTaskVoid SpawnAtInterval()
        {
            var prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefab.SetActive(false);
            prefab.AddComponent<Rigidbody>();

            while (true)
            {
                var spawnPosition = transform.position + Vector3.up * 5f;
                var clone = Instantiate(prefab, spawnPosition, Quaternion.identity);
                
                clone.SetActive(true);
                await UniTask.Delay(1000);
            }
        }

        private async UniTaskVoid LightBlinkLoop()
        {
            while (true)
            {
                _light.gameObject.SetActive(!_light.gameObject.activeSelf);
                await UniTask.Delay(1000);
            }
        }

        private async UniTaskVoid WaitForAnimation()
        {
            Tween.PositionX(transform, endValue: 10f, duration: 1.5f);
            await Tween.Scale(transform, endValue: 2f, duration: 0.5f, startDelay: 1);
            await Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f);
            await Tween.Delay(1);
            Debug.Log("Sequence completed");
        }

        private async UniTaskVoid WaitForPlayer()
        {
            Debug.Log("Press SPACE to open the door");
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            Debug.Log("Door Opened");
        }

        private async UniTaskVoid MakePizza()
        {
            Debug.Log("Putting pizza at oven");
            await UniTask.Delay(5000);
            Debug.Log("Pizza is Ready");
        }
    }
}