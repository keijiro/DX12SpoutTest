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
    public EventKicker()
      => _dataMem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(EventData)));

    public EventData Data
    {
        get => Marshal.PtrToStructure<EventData>(_dataMem);
        set => Marshal.StructureToPtr(value, _dataMem, false);
    }

    public void Dispose()
    {
        if (_dataMem != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_dataMem);
            _dataMem = IntPtr.Zero;
        }
    }

    public void IssuePluginEvent(EventID eventID)
    {
        if (_cmdBuffer == null)
            _cmdBuffer = new CommandBuffer();
        else
            _cmdBuffer.Clear();

        _cmdBuffer.IssuePluginEventAndData
          (Plugin.GetRenderEventCallback(), (int)eventID, _dataMem);

        Graphics.ExecuteCommandBuffer(_cmdBuffer);
    }

    static CommandBuffer _cmdBuffer;

    IntPtr _dataMem;
}
