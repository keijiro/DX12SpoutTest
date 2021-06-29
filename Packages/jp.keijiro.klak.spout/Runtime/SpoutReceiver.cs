using UnityEngine;

namespace Klak.Spout {

[ExecuteInEditMode]
[AddComponentMenu("Klak/Spout/Spout Receiver")]
public sealed partial class SpoutReceiver : MonoBehaviour
{
    #region Receiver plugin object

    Receiver _receiver;

    void ReleaseReceiver()
    {
        _receiver?.Dispose();
        _receiver = null;
    }

    #endregion

    #region Buffer texture object

    RenderTexture Buffer => PrepareBufferTexture();

    RenderTexture _buffer;

    RenderTexture PrepareBufferTexture()
    {
        var src = _receiver.Texture;
        if (src == null) return null;

        if (_buffer != null && !Utility.CompareSpecs(_buffer, src))
        {
            Utility.Destroy(_buffer);
            _buffer = null;
        }

        if (_buffer == null)
        {
            _buffer = new RenderTexture(src.width, src.height, 0);
            _buffer.hideFlags = HideFlags.DontSave;
        }

        return _buffer;
    }

    #endregion

    #region MonoBehaviour implementation

    void OnDisable()
      => ReleaseReceiver();

    void OnDestroy()
    {
        Utility.Destroy(_buffer);
        _buffer = null;
    }

    void Update()
    {
        // Receiver lazy initialization
        if (_receiver == null)
            _receiver = new Receiver(_sourceName);

        // Receiver plugin-side update
        _receiver.Update();

        // Do nothing further if no texture is ready yet.
        if (_receiver.Texture == null) return;

        // Received texture buffering
        Blitter.Blit(_receiver.Texture, receivedTexture);

        // Renderer override
        if (_targetRenderer != null)
            RendererOverride.SetTexture
              (_targetRenderer, _targetMaterialProperty, receivedTexture);
    }

    #endregion
}

} // namespace Klak.Spout
