using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Klak.Spout {

public static class SpoutManager
{
    public static string[] GetSourceNames()
    {
        IntPtr[] pointers;
        int count;
        Plugin.GetSenderNames(out pointers, out count);

        var names = new string[count];
        for (var i = 0; i < count; i++)
        {
            names[i] = Marshal.PtrToStringAnsi(pointers[i]);
            Marshal.FreeCoTaskMem(pointers[i]);
        }

        return names;
    }
}

} // namespace Klak.Spout
