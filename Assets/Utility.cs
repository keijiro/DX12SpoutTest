using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

static class Utility
{
    [DllImport("Plugin")] static extern IntPtr GetRenderEventCallback();

    static CommandBuffer _cmdBuffer;

    public static void IssuePluginEvent(int eventID, IntPtr ptr)
    {
        if (_cmdBuffer == null)
            _cmdBuffer = new CommandBuffer();
        else
            _cmdBuffer.Clear();

        _cmdBuffer.IssuePluginEventAndData
          (GetRenderEventCallback(), eventID, ptr);

        Graphics.ExecuteCommandBuffer(_cmdBuffer);
    }

    public static void IssuePluginEvent(int eventID)
      => IssuePluginEvent(eventID, IntPtr.Zero);
}
