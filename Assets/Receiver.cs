using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

sealed class Receiver : MonoBehaviour
{
    [DllImport("Plugin")] static extern IntPtr GetReceiverTexturePointer();

    Texture2D _texture;

    void OnDestroy()
    {
        Utility.IssuePluginEvent(2);

        if (_texture != null) Destroy(_texture);
    }

    void Update()
    {
        if (_texture != null) return;

        var ptr = GetReceiverTexturePointer();

        if (ptr == IntPtr.Zero)
        {
            Utility.IssuePluginEvent(0);
        }
        else
        {
            _texture = Texture2D.CreateExternalTexture
              (640, 360, TextureFormat.RGBA32, false, false, ptr);
            GetComponent<Renderer>().material.mainTexture = _texture;
        }
    }
}
