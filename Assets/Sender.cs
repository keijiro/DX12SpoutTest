using UnityEngine;
using IntPtr = System.IntPtr;

sealed class Sender : MonoBehaviour
{
    [SerializeField] string _name = "DX12 Test";
    [SerializeField] Texture _source = null;

    IntPtr _instance;
    EventKicker _event;

    void Start()
    {
        _instance = Plugin.CreateSender(_name, _source.width, _source.height);

        _event = new EventKicker()
          { Data = new EventData(_instance, _source.GetNativeTexturePtr()) };

        _event.IssuePluginEvent(EventID.UpdateSender);
    }

    void OnDestroy()
    {
        _event.IssuePluginEvent(EventID.CloseSender);
    }

    void Update()
    {
        _event.IssuePluginEvent(EventID.UpdateSender);
    }
}
