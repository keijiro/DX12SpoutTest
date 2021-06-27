#pragma once

#include "Common.h"
#include "Sender.h"
#include "Receiver.h"

namespace KlakSpout {

enum EventID
  { event_updateSender,
    event_updateReceiver,
    event_closeSender,
    event_closeReceiver };

struct EventData
{
    union { Sender* sender;
            Receiver* receiver; };

    IUnknown* texture;
};

} // namespace KlakSpout
