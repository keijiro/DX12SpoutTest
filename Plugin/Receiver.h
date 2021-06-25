#pragma once

#include "Common.h"

namespace KlakSpout {

class Receiver final
{
public:

    Receiver(const char* name)
    {
        // Search the Spout name list.
        HANDLE handle;
        DWORD format;
        unsigned int w, h;
        auto res = _system->spout.CheckSender(name, w, h, handle, format);
        if (!res) return;

        std::printf("Sender found: %p, %d x %d\n", handle, w, h);

        // Handle -> D3D12Resource
        auto hres = _system->getD3D12Device()
          ->OpenSharedHandle(handle, IID_PPV_ARGS(&_texture));

        if (FAILED(hres))
            std::printf("OpenSharedHandle failed (%x)\n", hres);
        else
            std::puts("Receiver created");
    }

    ~Receiver()
    {
        _texture = nullptr;
    }

    void* getTexturePointer() { return _texture.Get(); }

private:

    WRL::ComPtr<ID3D12Resource> _texture;
};

} // namespace KlakSpout
