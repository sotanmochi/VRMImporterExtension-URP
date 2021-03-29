using System;

namespace VRM.Extension.Samples
{
    public interface IShaderSelectEventTrigger
    {
        event Action<string> OnTriggerShaderSelectEvent;
    }
}
