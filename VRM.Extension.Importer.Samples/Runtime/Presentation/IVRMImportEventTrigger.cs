using System;

namespace VRM.Extension.Samples
{
    public interface IVRMImportEventTrigger
    {
        event Action<string> OnTriggerVRMImportEvent;
    }
}
