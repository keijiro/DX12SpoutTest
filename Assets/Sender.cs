using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

sealed class Sender : MonoBehaviour
{
    [SerializeField] Texture _sourceTexture = null;

    [DllImport("Plugin")] static extern IntPtr GetSenderTexturePointer();

    void Start()
      => Utility.IssuePluginEvent(1, _sourceTexture.GetNativeTexturePtr());

    void OnDestroy()
      => Utility.IssuePluginEvent(3);
}
