using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

static class Plugin
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ReceiverData
    {
        public uint width, height;
        public IntPtr texturePointer;
    }

    [DllImport("Plugin")]
    public static extern IntPtr GetRenderEventCallback();

    [DllImport("Plugin")]
    public static extern IntPtr CreateSenderD3D12(string name, int width, int height);

    [DllImport("Plugin")]
    public static extern IntPtr CreateReceiverD3D12(string name);

    [DllImport("Plugin")]
    public static extern ReceiverData GetReceiverData(IntPtr receiver);
}
