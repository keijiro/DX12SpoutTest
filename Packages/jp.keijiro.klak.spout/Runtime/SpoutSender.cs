using UnityEngine;
using UnityEngine.Rendering;

namespace Klak.Spout {

//[ExecuteInEditMode]
[AddComponentMenu("Klak/Spout/Spout Sender")]
public sealed partial class SpoutSender : MonoBehaviour
{
    #region Sender plugin object

    Sender _sender;

    void ReleaseSender()
    {
        _sender?.Dispose();
        _sender = null;
    }

    #endregion

    Camera _attachedCamera;

    #region Buffer texture object

    RenderTexture _buffer;

    void PrepareBuffer(int width, int height)
    {
        if (_buffer != null &&
            (_buffer.width != width || _buffer.height != height))
        {
            Utility.Destroy(_buffer);
            _buffer = null;
        }

        if (_buffer == null)
        {
            _buffer = new RenderTexture(width, height, 0);
            _buffer.hideFlags = HideFlags.DontSave;
            _buffer.Create();
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void OnDisable()
      => ReleaseSender();

    void OnDestroy()
    {
        Utility.Destroy(_buffer);
        _buffer = null;
    }

    void OnCameraCapture(RenderTargetIdentifier source, CommandBuffer cb)
    {
        if (_attachedCamera == null) return;
        Blitter.Blit(cb, source, _buffer);
    }

    void Update()
    {
        if (_captureMethod == CaptureMethod.Texture)
        {
            PrepareBuffer(_sourceTexture.width, _sourceTexture.height);
            Blitter.Blit(_sourceTexture, _buffer);
        }

        if (_captureMethod == CaptureMethod.GameView)
        {
            PrepareBuffer(Screen.width, Screen.height);
            var temp = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            ScreenCapture.CaptureScreenshotIntoRenderTexture(temp);
            Graphics.Blit(temp, _buffer);
            RenderTexture.ReleaseTemporary(temp);
        }

        if (_captureMethod == CaptureMethod.Camera)
        {
            if (_sourceCamera != null && _attachedCamera == null)
            {
                _attachedCamera = _sourceCamera;
                PrepareBuffer(_sourceCamera.pixelWidth, _sourceCamera.pixelHeight);
                #if KLAK_SPOUT_HAS_SRP
                CameraCaptureBridge.AddCaptureAction(_attachedCamera, OnCameraCapture);
                #endif
            }
        }


        // Sender lazy initialization
        if (_sender == null) _sender = new Sender(_spoutName, _buffer);

        // Sender plugin-side update
        _sender.Update();
    }

    #endregion
}

} // namespace Klak.Spout
