using UnityEngine;

public class WebCamTextureManagerPC : MonoBehaviour
{
    public WebCamTexture WebCamTexture { get; private set; }
    
    [SerializeField] private int _cameraIndex;

    private void Start()
    {
        WebCamDevice myCamDevice = new WebCamDevice();
        WebCamDevice[] devices = WebCamTexture.devices;
        
        for (int i = 0; i < devices.Length; i++)
        {
            myCamDevice = devices[_cameraIndex];
        }
        
        WebCamTexture = new WebCamTexture(myCamDevice.name);
        WebCamTexture.Play();
    }
}
