#pragma once

#include <memory>
#include <d3d12.h>
#include <d3d11on12.h>
#include <wrl/client.h> // for ComPtr
#include "Spout/SpoutSenderNames.h"
#include "Unity/IUnityGraphics.h"
#include "Unity/IUnityGraphicsD3D12.h"

namespace KlakSpout {

namespace WRL = Microsoft::WRL;

class System final
{
public:

    spoutSenderNames spout;

    System(IUnityInterfaces* unity)
      : _unity(unity) {}

    IUnityGraphics* getGraphics() const
    {
        return _unity->Get<IUnityGraphics>();
    }

    WRL::ComPtr<ID3D12Device> getD3D12Device() const
    {
        return _unity->Get<IUnityGraphicsD3D12v6>()->GetDevice();
    }

    WRL::ComPtr<ID3D11On12Device> getD3D11On12Device()
    {
        if (!_d3d11_device) PrepareD3D11On12();
        WRL::ComPtr<ID3D11On12Device> d3d11on12;
        _d3d11_device.As(&d3d11on12);
        return d3d11on12;
    }

    WRL::ComPtr<ID3D11Device> getD3D11Device()
    {
        if (!_d3d11_device) PrepareD3D11On12();
        return _d3d11_device;
    }

    WRL::ComPtr<ID3D11DeviceContext> getD3D11Context()
    {
        if (!_d3d11_context) PrepareD3D11On12();
        return _d3d11_context;
    }

    void releaseD3DObjects()
    {
        _d3d11on12 = nullptr;
        _d3d11_device = nullptr;
        _d3d11_context = nullptr;
    }

private:

    IUnityInterfaces* _unity;

    WRL::ComPtr<ID3D11On12Device> _d3d11on12;
    WRL::ComPtr<ID3D11Device> _d3d11_device;
    WRL::ComPtr<ID3D11DeviceContext> _d3d11_context;

    void PrepareD3D11On12()
    {
        // Command queue array
        IUnknown* queues[]
          = { _unity->Get<IUnityGraphicsD3D12v6>()->GetCommandQueue() };

        // Create a D3D11-on-12 device.
        auto hres = D3D11On12CreateDevice
          (getD3D12Device().Get(), 0, nullptr, 0,
           queues, 1, 0, &_d3d11_device, &_d3d11_context, nullptr);

        if (FAILED(hres))
            std::printf("D3D11On12CreateDevice failed (%x)\n", hres);
    }
};

inline std::unique_ptr<System> _system;

} // namespace KlakSpout
