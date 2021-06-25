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
    public static extern IntPtr CreateSender(string name, int width, int height);

    [DllImport("Plugin")]
    public static extern IntPtr CreateReceiver(string name);

    [DllImport("Plugin")]
    public static extern ReceiverData GetReceiverData(IntPtr receiver);
}
