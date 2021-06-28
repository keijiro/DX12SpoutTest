using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Klak.Spout {

//[ExecuteInEditMode]
[AddComponentMenu("Klak/Spout/Spout Receiver")]
public sealed class SpoutReceiver : MonoBehaviour
{
    [SerializeField] string _sourceName = "Spout Demo Sender";

    IntPtr _instance;
    EventKicker _event;

    Texture2D _texture;

    void Start()
    {
        _instance = Plugin.CreateReceiverD3D12(_sourceName);
        _event = new EventKicker(new EventData(_instance));

        _event.IssuePluginEvent(EventID.UpdateReceiver);
    }

    void OnDestroy()
    {
        _event.IssuePluginEvent(EventID.CloseReceiver);
        _event.Dispose();

        if (_texture != null) Destroy(_texture);
    }

    void Update()
    {
        if (_texture == null)
        {
            var data = Plugin.GetReceiverData(_instance);

            if (data.texturePointer != IntPtr.Zero)
            {
                _texture = Texture2D.CreateExternalTexture
                  ((int)data.width, (int)data.height, TextureFormat.RGBA32,
                   false, false, data.texturePointer);

                GetComponent<Renderer>().material.mainTexture = _texture;
            }
        }

        _event.IssuePluginEvent(EventID.UpdateReceiver);
    }
}

} // namespace Klak.Spout
