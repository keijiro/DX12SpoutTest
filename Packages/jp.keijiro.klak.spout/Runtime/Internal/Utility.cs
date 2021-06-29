using UnityEngine;

namespace Klak.Spout {

static class RendererOverride
{
    static MaterialPropertyBlock _block;

    public static void SetTexture
      (Renderer renderer, string property, Texture texture)
    {
        if (_block == null) _block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(_block);
        _block.SetTexture(property, texture);
        renderer.SetPropertyBlock(_block);
    }
}

static class Blitter
{
    public static void Blit(Texture source, RenderTexture destination)
      => RunPass(source, destination, 0);

    public static void BlitClearAlpha(Texture source, RenderTexture destination)
      => RunPass(source, destination, 1);

    public static void BlitToLinear(Texture source, RenderTexture destination)
      => RunPass(source, destination, 2);

    static Material _material;

    static void RunPass(Texture source, RenderTexture destination, int pass)
    {
        if (_material == null)
        {
            _material = new Material(Shader.Find("Hidden/Klak/Spout/Blit"));
            _material.hideFlags = HideFlags.DontSave;
        }
        Graphics.Blit(source, destination, _material, pass);
    }

}

static class Utility
{
    public static bool CompareSpecs(Texture a, Texture b)
      => a.width == b.width && a.height == b.height;

    public static void Destroy(Object obj)
    {
        if (obj == null) return;

        if (Application.isPlaying)
            Object.Destroy(obj);
        else
            Object.DestroyImmediate(obj);
    }
}

} // namespace Klak.Spout
