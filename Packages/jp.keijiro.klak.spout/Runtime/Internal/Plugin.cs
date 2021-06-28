using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Klak.Spout {

static class Plugin
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ReceiverData
    {
        public uint width, height;
        public IntPtr texturePointer;
    }

    [DllImport("KlakSpout")]
    public static extern IntPtr GetRenderEventCallback();

    [DllImport("KlakSpout")]
    public static extern IntPtr CreateSenderD3D12(string name, int width, int height);

    [DllImport("KlakSpout")]
    public static extern IntPtr CreateReceiverD3D12(string name);

    [DllImport("KlakSpout")]
    public static extern ReceiverData GetReceiverData(IntPtr receiver);

    [DllImport("KlakSpout")]
    public static extern void GetSenderNames
      ([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
       out IntPtr[] names, out int count);
}

} // namespace Klak.Spout
