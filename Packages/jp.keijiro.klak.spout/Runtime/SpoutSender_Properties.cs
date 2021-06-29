using UnityEngine;

namespace Klak.Spout {

public enum CaptureMethod { GameView, Camera, Texture }

partial class SpoutSender
{
    #region Spout source

    [SerializeField] string _sourceName = "Spout Sender";

    public string sourceName
      { get => _sourceName;
        set => ChangeSourceName(value); }

    #endregion

    #region Format option

    [SerializeField] bool _enableAlpha = false;

    public bool enableAlpha
      { get => _enableAlpha;
        set => _enableAlpha = value; }

    #endregion

    #region Capture target

    [SerializeField] CaptureMethod _captureMethod = CaptureMethod.GameView;

    public CaptureMethod captureMethod
      { get => _captureMethod;
        set => ChangeCaptureMethod(value); }

    [SerializeField] Camera _sourceCamera = null;

    public Camera sourceCamera
      { get => _sourceCamera;
        set => ChangeSourceCamera(value); }

    [SerializeField] Texture _sourceTexture = null;

    public Texture sourceTexture
      { get => _sourceTexture;
        set => _sourceTexture = value; }

    #endregion

    void ChangeSourceName(string name) {}
    void ChangeCaptureMethod(CaptureMethod method) {}
    void ChangeSourceCamera(Camera camera) {}
}

} // namespace Klak.Spout
