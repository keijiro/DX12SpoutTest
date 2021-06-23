using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

sealed class Test : MonoBehaviour
{
    #region Native plugin interface

    [DllImport("Plugin")] static extern IntPtr GetRenderEventCallback();
    [DllImport("Plugin")] static extern IntPtr GetTexturePointer();
    [DllImport("Plugin")] static extern uint GetTextureWidth();
    [DllImport("Plugin")] static extern uint GetTextureHeight();

    #endregion

    #region Private objects

    CommandBuffer _pluginCommand;
    Texture2D _texture;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _pluginCommand = new CommandBuffer();
        _pluginCommand.IssuePluginEventAndData(GetRenderEventCallback(), 0, IntPtr.Zero);
    }

    void OnDestroy()
    {
        if (_texture != null) Destroy(_texture);
    }

    void Update()
    {
        if (_texture != null) return;

        var ptr = GetTexturePointer();

        if (ptr == IntPtr.Zero)
        {
            Graphics.ExecuteCommandBuffer(_pluginCommand);
        }
        else
        {
            _texture = Texture2D.CreateExternalTexture
              ((int)GetTextureWidth(), (int)GetTextureHeight(),
               TextureFormat.RGBA32, false, false, ptr);
            GetComponent<Renderer>().material.mainTexture = _texture;
        }
    }

    #endregion
}
