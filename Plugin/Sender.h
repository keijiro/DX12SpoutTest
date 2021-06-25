#pragma once

#include "Common.h"

namespace Spout {

class Sender final
{
public:

    Sender(const char* name)
    {
        auto& g = *SharedObjects::global;

        // D3D12 device
        auto d3d12 = g.unity->Get<IUnityGraphicsD3D12v6>()->GetDevice();

        // Command queue array
        IUnknown* queues[] =
          { g.unity->Get<IUnityGraphicsD3D12v6>()->GetCommandQueue() };

        // D3D11 on 12 device
        auto hres = D3D11On12CreateDevice
          (d3d12, 0, nullptr, 0, queues, 1, 0, &g.d3d11_dev, &g.d3d11_ctx, nullptr);

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
        hres = g.d3d11_dev->CreateTexture2D(&desc, nullptr, &_texture);

        if (FAILED(hres))
        {
            std::printf("CreateTexture2D failed (%x)\n", hres);
            return;
        }

        // Retrieve the texture handle.
        HANDLE handle;
        WRL::ComPtr<IDXGIResource> resource;
        _texture.As(&resource);
        resource->GetSharedHandle(&handle);

        // Create a Spout sender object for the shared texture.
        auto res = g.spout->CreateSender(name, 640, 360, handle, desc.Format);
        std::puts(res ? "Sender activated" : "CreateSender failed");
    }

    ~Sender()
    {
        auto& g = *SharedObjects::global;

        _texture = nullptr;
        g.d3d11_dev = nullptr;
        g.d3d11_ctx = nullptr;
    }

    void update(ID3D12Resource* source)
    {
        auto& g = *SharedObjects::global;

        // ID3D11On12Device retrieval
        WRL::ComPtr<ID3D11On12Device> d3d11on12;
        g.d3d11_dev.As(&d3d11on12);

        // Wrapping: D3D12 -> D3D11
        D3D11_RESOURCE_FLAGS flags = {};
        WRL::ComPtr<ID3D11Resource> wrap;
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
        g.d3d11_ctx->CopyResource(_texture.Get(), wrap.Get());
        d3d11on12->ReleaseWrappedResources(wrap.GetAddressOf(), 1);
        g.d3d11_ctx->Flush();
    }

    void* getTexturePointer() { return _texture.Get(); }

private:

    WRL::ComPtr<ID3D11Texture2D> _texture;
};

} // namespace Spout
