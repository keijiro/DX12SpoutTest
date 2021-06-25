#pragma once

#include "Common.h"

namespace KlakSpout {

class Receiver final
{
public:

    struct InteropData
    {
        unsigned int width, height;
        void* texture_pointer;
    };

    Receiver(const char* name)
      : _name(name) {}

    ~Receiver()
    {
        _texture = nullptr;
    }

    void update()
    {
        if (_texture) return;

        // Search the Spout name list.
        HANDLE handle;
        DWORD format;
        auto res = _system->spout
          .CheckSender(_name.c_str(), _width, _height, handle, format);
        if (!res) return;

        // Handle -> D3D12Resource
        auto hres = _system->getD3D12Device()
          ->OpenSharedHandle(handle, IID_PPV_ARGS(&_texture));

        if (FAILED(hres))
            std::printf("OpenSharedHandle failed (%x)\n", hres);
        else
            std::puts("Receiver created");
    }

    InteropData getInteropData() const
    {
        return InteropData
          { .width = _width, .height = _height,
            .texture_pointer = _texture.Get() };
    }

private:

    std::string _name;
    unsigned int _width, _height;
    WRL::ComPtr<ID3D12Resource> _texture;
};

} // namespace KlakSpout
