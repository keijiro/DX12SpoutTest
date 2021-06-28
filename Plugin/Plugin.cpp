#include "Common.h"
#include "Event.h"
#include "Receiver.h"
#include "Sender.h"
#include "System.h"

using namespace KlakSpout;

namespace {

//
// Graphics device event callback
//
void UNITY_INTERFACE_API
  OnGraphicsDeviceEvent(UnityGfxDeviceEventType event_type)
{
    if (event_type == kUnityGfxDeviceEventShutdown) _system->shutdown();
}

//
// Render event (via IssuePluginEvent) callback
//
void UNITY_INTERFACE_API
  OnRenderEvent(int event_id, void* event_data)
{
    auto data = reinterpret_cast<const EventData*>(event_data);
    if (event_id == event_updateSender  ) data->sender->update(data->texture);
    if (event_id == event_updateReceiver) data->receiver->update();
    if (event_id == event_closeSender  ) delete data->sender;
    if (event_id == event_closeReceiver) delete data->receiver;
}

} // anonymous namespace

//
// Unity low-level native plugin interface
//

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
  UnityPluginLoad(IUnityInterfaces* interfaces)
{
    // Open a new console for stdout.
    //FILE * pConsole;
    //AllocConsole();
    //freopen_s(&pConsole, "CONOUT$", "wb", stdout);

    // System object instantiation, callback registration
    _system = std::make_unique<System>(interfaces);
    _system->getGraphics()->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
    // System object destruction
    _system->getGraphics()->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
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
