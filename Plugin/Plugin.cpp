#include "Common.h"
#include "Sender.h"
#include "Receiver.h"

using namespace KlakSpout;

namespace {

std::unique_ptr<Sender> _sender;
std::unique_ptr<Receiver> _receiver;

//
// Graphics device event callback
//
void UNITY_INTERFACE_API
  OnGraphicsDeviceEvent(UnityGfxDeviceEventType event_type)
{
    // We only deal with D3D12.
    auto renderer_type = _system->unity->Get<IUnityGraphics>()->GetRenderer();
    if (renderer_type != kUnityGfxRendererD3D12) return;

    if (event_type == kUnityGfxDeviceEventInitialize)
    {
        // Device initialization event:
        // Initialize the Spout name table.
        _system->spout = std::make_unique<spoutSenderNames>();
    }
    else if (event_type == kUnityGfxDeviceEventShutdown)
    {
        // Device shutdown event:
        // Release the Spout name table.
        _system->spout.reset();
    }
}

//
// Render event (via IssuePluginEvent) callback
//
void UNITY_INTERFACE_API OnRenderEvent(int event_id, void* data)
{
    if (event_id == 0)
    {
        // Event 0: Receiver run
        if (!_receiver)
        {
            _receiver = std::make_unique<Receiver>("Spout Demo Sender");
        }
    }
    else if (event_id == 1)
    {
        // Event 1: Sender run
        if (!_sender)
            _sender = std::make_unique<Sender>("DX12 Test");

        _sender->update(reinterpret_cast<ID3D12Resource*>(data));
    }
    else if (event_id == 2)
    {
        // Event 2: Receiver shutdown
        _receiver.reset();
    }
    else if (event_id == 3)
    {
        // Event 3: Sender shutdown
        _sender.reset();
    }
}

} // anonymous namespace

//
// Unity low-level native plugin interface
//

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
  UnityPluginLoad(IUnityInterfaces* interfaces)
{
    // Open a new console for stdout.
    FILE * pConsole;
    AllocConsole();
    freopen_s(&pConsole, "CONOUT$", "wb", stdout);

    // Instantiate the common system object.
    _system = std::make_unique<System>();

    // Grab the plugin interface pointer.
    _system->unity = interfaces;

    // Register the graphics device event callback.
    interfaces->Get<IUnityGraphics>()
      ->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

    // We must manually trigger the initialization event.
    OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
    // Unregister the graphics device event callback.
    _system->unity->Get<IUnityGraphics>()
      ->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);

    // Release the common system object.
    _system.reset();
}

//
// Plugin functions
//

extern "C" UnityRenderingEventAndData UNITY_INTERFACE_EXPORT GetRenderEventCallback()
{
    return OnRenderEvent;
}

extern "C" void UNITY_INTERFACE_EXPORT * GetReceiverTexturePointer()
{
    return _receiver ? _receiver->getTexturePointer() : nullptr;
}

extern "C" void UNITY_INTERFACE_EXPORT * GetSenderTexturePointer()
{
    return _sender ? _sender->getTexturePointer() : nullptr;
}
