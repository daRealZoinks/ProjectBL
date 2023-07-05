using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraSettings : MonoBehaviour
{
    private AntialiasingMode _antialiasingMode;
    private AntialiasingQuality _antialiasingQuality;
    private Camera _cam;
    private UniversalAdditionalCameraData _camData;

    public AntialiasingMode AntialiasingMode
    {
        get => _antialiasingMode;
        set
        {
            _antialiasingMode = value;
            _camData.antialiasing = AntialiasingMode;
        }
    }

    public AntialiasingQuality AntialiasingQuality
    {
        get => _antialiasingQuality;
        set
        {
            _antialiasingQuality = value;
            _camData.antialiasingQuality = AntialiasingQuality;
        }
    }

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _camData = _cam.GetUniversalAdditionalCameraData();

        // TODO: get the antialiasing settings from somewhere
    }
}