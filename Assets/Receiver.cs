using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

sealed class Receiver : MonoBehaviour
{
    [DllImport("Plugin")] static extern IntPtr GetReceiverTexturePointer();

    Texture2D _texture;

    void Start()
      => Utility.IssuePluginEvent(0);

    void OnDestroy()
    {
        if (_texture != null)
        {
            Utility.IssuePluginEvent(2);
            Destroy(_texture);
        }
    }

    void Update()
    {
        if (_texture != null) return;

        var ptr = GetReceiverTexturePointer();
        if (ptr == IntPtr.Zero) return;

        _texture = Texture2D.CreateExternalTexture
          (640, 360, TextureFormat.RGBA32, false, false, ptr);

        GetComponent<Renderer>().material.mainTexture = _texture;
    }
}
