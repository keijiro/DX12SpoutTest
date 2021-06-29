using UnityEngine;
using IntPtr = System.IntPtr;

namespace Klak.Spout {

//[ExecuteInEditMode]
[AddComponentMenu("Klak/Spout/Spout Sender")]
public sealed partial class SpoutSender : MonoBehaviour
{
    #region Sender plugin object

    Sender _sender;

    void ReleaseSender()
    {
        _sender?.Dispose();
        _sender = null;
    }

    #endregion

    #region MonoBehaviour implementation

    void OnDisable()
      => ReleaseSender();

    void Update()
    {
        // Sender lazy initialization
        if (_sender == null)
            _sender = new Sender(_sourceName, _sourceTexture);

        // Sender plugin-side update
        _sender.Update();
    }

    #endregion
}

} // namespace Klak.Spout
