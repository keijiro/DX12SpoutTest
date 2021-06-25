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

    IUnityInterfaces* unity;

    WRL::ComPtr<ID3D11Device> d3d11_dev;
    WRL::ComPtr<ID3D11DeviceContext> d3d11_ctx;

    std::unique_ptr<spoutSenderNames> spout;
};

inline std::unique_ptr<System> _system;

} // namespace KlakSpout
