using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

static class Plugin
{
    [DllImport("Plugin")]
    public static extern IntPtr GetRenderEventCallback();

    [DllImport("Plugin")]
    public static extern IntPtr CreateSender(string name, int width, int height);

    [DllImport("Plugin")]
    public static extern IntPtr CreateReceiver(string name);

    [DllImport("Plugin")]
    public static extern IntPtr GetReceiverTexturePointer(IntPtr receiver);
}
