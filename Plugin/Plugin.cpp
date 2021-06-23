#include <memory>
#include <d3d12.h>
#include <wrl/client.h> // for ComPtr
#include "Spout/SpoutSenderNames.h"
#include "Unity/IUnityGraphics.h"
#include "Unity/IUnityGraphicsD3D12.h"

using Microsoft::WRL::ComPtr;

namespace {

// Unity native plugin interface
IUnityInterfaces* _unity;

// D3D12 objects
ComPtr<ID3D12Resource> _recv_texture;
ComPtr<ID3D12Resource> _send_texture;

// Spout objects
std::unique_ptr<spoutSenderNames> _name_list;

// Predefined names
const char* _recv_target = "Spout Demo Sender";
const char* _send_name = "DX12 Test Sender";

//
// Callback functions
//

// Graphics device events from the Unity graphics system
void UNITY_INTERFACE_API
  OnGraphicsDeviceEvent(UnityGfxDeviceEventType event_type)
{
    auto renderer_type = _unity->Get<IUnityGraphics>()->GetRenderer();
    if (renderer_type != kUnityGfxRendererD3D12) return;

    if (event_type == kUnityGfxDeviceEventInitialize)
    {
        // Device initialization event
        _name_list = std::make_unique<spoutSenderNames>();
    }
    else if (event_type == kUnityGfxDeviceEventShutdown)
    {
        // Device shutdown event
        _name_list.reset();
        _recv_texture = nullptr;
        _send_texture = nullptr;
    }
}

// Render events from Unity user scripts
void UNITY_INTERFACE_API OnRenderEvent(int event_id, void* data)
{
    if (event_id != 0) return;
    if (_recv_texture) return;

    auto device = _unity->Get<IUnityGraphicsD3D12v6>()->GetDevice();

    HANDLE handle;
    DWORD format;
    unsigned int w, h;
    auto res = _name_list->CheckSender(_recv_target, w, h, handle, format);
    if (!res) return;

    device->OpenSharedHandle(handle, IID_PPV_ARGS(&_recv_texture));
}

} // anonymous namespace

//
// Unity low-level native plugin interface
//

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
  UnityPluginLoad(IUnityInterfaces* interfaces)
{
    _unity = interfaces;

    // Graphics device event callback setup
    _unity->Get<IUnityGraphics>()
      ->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

    // We must manually trigger the initialization event.
    OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
    // Graphics device event callback cleanup
    _unity->Get<IUnityGraphics>()
      ->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);

    _unity = nullptr;
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
    return _recv_texture.Get();
}
