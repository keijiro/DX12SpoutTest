using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

sealed class Receiver : MonoBehaviour
{
    [SerializeField] string _targetName = "Spout Demo Sender";

    IntPtr _instance;
    EventKicker _event;

    Texture2D _texture;

    void Start()
    {
        _instance = Plugin.CreateReceiver(_targetName);
        _event = new EventKicker() { Data = new EventData(_instance) };

        _event.IssuePluginEvent(EventID.UpdateReceiver);
    }

    void OnDestroy()
    {
        _event.IssuePluginEvent(EventID.CloseReceiver);

        if (_texture != null) Destroy(_texture);
    }

    void Update()
    {
        if (_texture == null)
        {
            var ptr = Plugin.GetReceiverTexturePointer(_instance);

            if (ptr != IntPtr.Zero)
            {
                _texture = Texture2D.CreateExternalTexture
                  (640, 360, TextureFormat.RGBA32, false, false, ptr);

                GetComponent<Renderer>().material.mainTexture = _texture;
            }
        }

        _event.IssuePluginEvent(EventID.UpdateReceiver);
    }
}
