#include "Event.h"

using namespace KlakSpout;

namespace {

//
// Graphics device event callback
//
void UNITY_INTERFACE_API
  OnGraphicsDeviceEvent(UnityGfxDeviceEventType event_type)
{
    // Device shutdown event
    if (event_type == kUnityGfxDeviceEventShutdown)
        _system->releaseD3DObjects();
}

//
// Render event (via IssuePluginEvent) callback
//
void UNITY_INTERFACE_API OnRenderEvent(int event_id, void* event_data)
{
    auto data = reinterpret_cast<const EventData*>(event_data);

    if (event_id == event_updateSender)
    {
        if (data->receiver) data->sender->update(data->texture);
    }
    else if (event_id == event_updateReceiver)
    {
        if (data->sender) data->receiver->update();
    }
    else if (event_id == event_closeSender)
    {
        if (data->receiver) delete data->sender;
    }
    else if (event_id == event_closeReceiver)
    {
        if (data->sender) delete data->receiver;
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
    _system = std::make_unique<System>(interfaces);

    // Register the graphics device event callback.
    _system->getGraphics()->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

    // We must manually trigger the initialization event.
    OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
    // Unregister the graphics device event callback.
    _system->getGraphics()->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);

    // Release the common system object.
    _system.reset();
}

//
// Plugin functions
//

extern "C" UnityRenderingEventAndData UNITY_INTERFACE_EXPORT
  GetRenderEventCallback()
{
    return OnRenderEvent;
}

extern "C" Sender UNITY_INTERFACE_EXPORT *
  CreateSenderD3D12(const char* name, int width, int height)
{
    return new Sender(name, width, height);
}

extern "C" Receiver UNITY_INTERFACE_EXPORT *
  CreateReceiverD3D12(const char* name)
{
    return new Receiver(name);
}

extern "C" Receiver::InteropData UNITY_INTERFACE_EXPORT
  GetReceiverData(Receiver* receiver)
{
    return receiver->getInteropData();
}
