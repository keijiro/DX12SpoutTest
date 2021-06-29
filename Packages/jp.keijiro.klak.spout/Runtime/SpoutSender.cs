using UnityEngine;
using IntPtr = System.IntPtr;

namespace Klak.Spout {

//[ExecuteInEditMode]
[AddComponentMenu("Klak/Spout/Spout Sender")]
public sealed class SpoutSender : MonoBehaviour
{
    [SerializeField] string _name = "DX12 Test";
    [SerializeField] Texture _source = null;

    IntPtr _instance;
    EventKicker _event;

    void Start()
    {
        _instance = Plugin.CreateSender(_name, _source.width, _source.height);

        _event = new EventKicker
          (new EventData(_instance, _source.GetNativeTexturePtr()));

        _event.IssuePluginEvent(EventID.UpdateSender);
    }

    void OnDestroy()
    {
        _event.IssuePluginEvent(EventID.CloseSender);
        _event.Dispose();
    }

    void Update()
    {
        _event.IssuePluginEvent(EventID.UpdateSender);
    }
}

} // namespace Klak.Spout
