using UnityEngine;

namespace Klak.Spout {

partial class SpoutReceiver
{
    #region Spout source

    [SerializeField] string _sourceName = null;

    public string sourceName
      { get => _sourceName;
        set => ChangeSourceName(value); }

    #endregion

    #region Destination settings

    [SerializeField] RenderTexture _targetTexture = null;

    public RenderTexture targetTexture
      { get => _targetTexture;
        set => _targetTexture = value; }

    [SerializeField] Renderer _targetRenderer = null;

    public Renderer targetRenderer
      { get => _targetRenderer;
        set => _targetRenderer = value; }

    [SerializeField] string _targetMaterialProperty = null;

    public string targetMaterialProperty
      { get => _targetMaterialProperty;
        set => _targetMaterialProperty = value; }

    #endregion

    #region Runtime property

    public RenderTexture receivedTexture
      => _targetTexture != null ? _targetTexture : Buffer;

    #endregion
}

} // namespace Klak.Spout
