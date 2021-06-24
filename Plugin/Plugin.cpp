#include <cstdio>
#include <cstddef>
#include <memory>

#include <d3d12.h>
#include <d3d11on12.h>

#include <wrl/client.h> // for ComPtr

#include "Spout/SpoutSenderNames.h"

#include "Unity/IUnityGraphics.h"
#include "Unity/IUnityGraphicsD3D12.h"

using Microsoft::WRL::ComPtr;

namespace {

// Unity native plugin interface
IUnityInterfaces* _unity;

// Texture resources
ComPtr<ID3D12Resource> _recv_texture; // receiver on D3D12
ComPtr<ID3D11Texture2D> _send_texture;  // sender on D3D11

// D3D11On12 objects
ComPtr<ID3D11Device> _d3d11_dev;
ComPtr<ID3D11DeviceContext> _d3d11_ctx;

// Spout objects
std::unique_ptr<spoutSenderNames> _spout;

// Predefined names
//const char* _recv_target = "DX12 Test";
const char* _recv_target = "Spout Demo Sender";
const char* _send_name = "DX12 Test";

//
// Sender
//

void InitializeSender()
{
    // D3D12 device
    auto d3d12 = _unity->Get<IUnityGraphicsD3D12v6>()->GetDevice();

    // Command queue array
    IUnknown* queues[] =
      { _unity->Get<IUnityGraphicsD3D12v6>()->GetCommandQueue() };

    // D3D11 on 12 device
    auto hres = D3D11On12CreateDevice
      (d3d12, 0, nullptr, 0, queues, 1, 0, &_d3d11_dev, &_d3d11_ctx, nullptr);

    if (FAILED(hres))
    {
        std::printf("D3D11On12CreateDevice failed (%x)\n", hres);
        return;
    }

    // Make a Spout-compatible texture description.
    D3D11_TEXTURE2D_DESC desc = {};
    desc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    desc.Width = 640;
    desc.Height = 360;
    desc.MipLevels = 1;
    desc.ArraySize = 1;
    desc.SampleDesc.Count = 1;
    desc.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
    desc.MiscFlags = D3D11_RESOURCE_MISC_SHARED;

    // Create a shared texture.
    hres = _d3d11_dev->CreateTexture2D(&desc, nullptr, &_send_texture);

    if (FAILED(hres))
    {
        std::printf("CreateTexture2D failed (%x)\n", hres);
        return;
    }

    // Retrieve the texture handle.
    HANDLE handle;
    ComPtr<IDXGIResource> resource;
    _send_texture.As(&resource);
    resource->GetSharedHandle(&handle);

    // Create a Spout sender object for the shared texture.
    auto res = _spout->CreateSender(_send_name, 640, 360, handle, desc.Format);
    std::puts(res ? "Sender activated" : "CreateSender failed");
}

void ReleaseSender()
{
    _send_texture = nullptr;
    _d3d11_dev = nullptr;
    _d3d11_ctx = nullptr;
}

void UpdateSender(ID3D12Resource* source)
{
    // ID3D11On12Device retrieval
    ComPtr<ID3D11On12Device> d3d11on12;
    _d3d11_dev.As(&d3d11on12);

    // Wrapping: D3D12 -> D3D11
    D3D11_RESOURCE_FLAGS flags = {};
    ComPtr<ID3D11Resource> wrap;
    auto res = d3d11on12->CreateWrappedResource
      (source, &flags,
       D3D12_RESOURCE_STATE_COPY_SOURCE,
       D3D12_RESOURCE_STATE_PRESENT,
       IID_PPV_ARGS(&wrap));

    if (FAILED(res))
    {
        std::printf("CreateWrappedResource failed (%x)\n", res);
        return;
    }

    // Texture copy
    d3d11on12->AcquireWrappedResources(wrap.GetAddressOf(), 1);
	_d3d11_ctx->CopyResource(_send_texture.Get(), wrap.Get());
	d3d11on12->ReleaseWrappedResources(wrap.GetAddressOf(), 1);
    _d3d11_ctx->Flush();
}

//
// Receiver
//

void InitializeReceiver()
{
    // Search the Spout name list.
    HANDLE handle;
    DWORD format;
    unsigned int w, h;
    auto res = _spout->CheckSender(_recv_target, w, h, handle, format);
    if (!res) return;

    std::printf("Sender found: %p, %d x %d\n", handle, w, h);

    // Handle -> D3D12Resource
    auto d3d12 = _unity->Get<IUnityGraphicsD3D12v6>()->GetDevice();
    auto hres = d3d12->OpenSharedHandle(handle, IID_PPV_ARGS(&_recv_texture));

    if (FAILED(hres))
        std::printf("OpenSharedHandle failed (%x)\n", hres);
    else
        std::puts("Receiver created");
}

void ReleaseReceiver()
{
    _recv_texture = nullptr;
}

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
        _spout = std::make_unique<spoutSenderNames>();
    }
    else if (event_type == kUnityGfxDeviceEventShutdown)
    {
        // Device shutdown event
        _spout.reset();
    }
}

// Render events from Unity user scripts
void UNITY_INTERFACE_API OnRenderEvent(int event_id, void* data)
{
    if (event_id == 0)
    {
        if (!_recv_texture) InitializeReceiver();
    }
    else if (event_id == 1)
    {
        if (!_send_texture) InitializeSender();
        if (_send_texture) UpdateSender(reinterpret_cast<ID3D12Resource*>(data));
    }
    else if (event_id == 2)
    {
        if (_recv_texture) ReleaseReceiver();
    }
    else if (event_id == 3)
    {
        if (_send_texture) ReleaseSender();
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

    // Grab the plugin interface pointer.
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

    // Forget the plugin interface pointer.
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

extern "C" void UNITY_INTERFACE_EXPORT * GetSenderTexturePointer()
{
    return _send_texture.Get();
}
