using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

public class MicRecorder : MonoBehaviour
{
    public bool IsReady() => isRecordingAvailable;
    public string GetLastFilePath() => filePath;
    public int duration = 10; // seconds
    public int sampleRate = 44100;
    
    private AudioClip recordedClip;
    private string micDevice;
    private string filePath;
    private bool isRecordingAvailable;
    
    void Start()
    {
        micDevice = Microphone.devices[0];
    }

    [Button]
    public void StartRecording()
    {
        recordedClip = Microphone.Start(micDevice, false, duration, sampleRate);
        Debug.Log("Recording...");
    }

    [Button]
    public void StopRecording()
    {
        Microphone.End(micDevice);
        Debug.Log("Recording stopped.");

        SaveWav("recorded_audio", recordedClip);
    }

    void SaveWav(string filename, AudioClip clip)
    {
        filePath = Path.Combine(Application.persistentDataPath, filename + ".wav");
        if (SavWav.Save(filename, clip))
        {
            isRecordingAvailable = true;
            Debug.Log("Saved WAV to: " + filePath);
        }
        else
        {
            isRecordingAvailable = false;
        }
    }

}