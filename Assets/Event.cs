using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System;

enum EventID
{
    UpdateSender,
    UpdateReceiver,
    CloseSender,
    CloseReceiver
}

[StructLayout(LayoutKind.Sequential)]
struct EventData
{
    public IntPtr instancePointer;
    public IntPtr texturePointer;

    public EventData(IntPtr instance, IntPtr texture)
    {
        instancePointer = instance;
        texturePointer = texture;
    }

    public EventData(IntPtr instance)
    {
        instancePointer = instance;
        texturePointer = IntPtr.Zero;
    }
}

class EventKicker : IDisposable
{
    public EventKicker(EventData data)
      => _dataMem = GCHandle.Alloc(data, GCHandleType.Pinned);

    public void Dispose()
      => _dataMem.Free();

    public void IssuePluginEvent(EventID eventID)
    {
        if (_cmdBuffer == null)
            _cmdBuffer = new CommandBuffer();
        else
            _cmdBuffer.Clear();

        _cmdBuffer.IssuePluginEventAndData
          (Plugin.GetRenderEventCallback(),
           (int)eventID, _dataMem.AddrOfPinnedObject());

        Graphics.ExecuteCommandBuffer(_cmdBuffer);
    }

    static CommandBuffer _cmdBuffer;
    GCHandle _dataMem;
}
