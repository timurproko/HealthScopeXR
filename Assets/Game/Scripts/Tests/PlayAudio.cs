using Sirenix.OdinInspector;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    [Button]
    private void Play()
    {
        _audioSource.PlayOneShot(_audioClip);
    }
}
