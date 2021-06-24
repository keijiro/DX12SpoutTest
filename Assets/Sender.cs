using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

sealed class Sender : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] Texture _sourceTexture = null;

    #endregion

    #region Native plugin interface

    [DllImport("Plugin")] static extern IntPtr GetRenderEventCallback();
    [DllImport("Plugin")] static extern IntPtr GetSenderTexturePointer();

    #endregion

    #region Private objects

    CommandBuffer _pluginCommand;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _pluginCommand = new CommandBuffer();
        _pluginCommand.IssuePluginEventAndData
          (GetRenderEventCallback(), 1, _sourceTexture.GetNativeTexturePtr());
        Graphics.ExecuteCommandBuffer(_pluginCommand);
    }

    #endregion
}
