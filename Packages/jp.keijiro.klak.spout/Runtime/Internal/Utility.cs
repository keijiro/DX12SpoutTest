using UnityEngine;
using UnityEngine.Rendering;
using RTID = UnityEngine.Rendering.RenderTargetIdentifier;

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
    public static void Blit(Texture src, RenderTexture dst)
      => Graphics.Blit(src, dst, GetMaterial(), 0);

    public static void Blit(CommandBuffer cb, RTID src, RTID dst)
      => cb.Blit(src, dst, GetMaterial(), 0);

    public static void BlitClearAlpha(Texture src, RenderTexture dst)
      => Graphics.Blit(src, dst, GetMaterial(), 1);

    public static void BlitClearAlpha(CommandBuffer cb, RTID src, RTID dst)
      => cb.Blit(src, dst, GetMaterial(), 1);

    public static void BlitToLinear(Texture src, RenderTexture dst)
      => Graphics.Blit(src, dst, GetMaterial(), 2);

    public static void BlitToLinear(CommandBuffer cb, RTID src, RTID dst)
      => cb.Blit(src, dst, GetMaterial(), 2);

    static Material _material;

    static Material GetMaterial()
    {
        if (_material == null)
        {
            _material = new Material(Shader.Find("Hidden/Klak/Spout/Blit"));
            _material.hideFlags = HideFlags.DontSave;
        }
        return _material;
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
