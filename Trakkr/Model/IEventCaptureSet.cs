using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Trakkr.Model
{
    public interface IEventCaptureSet
    {
        IEvent Next();

        IEvent Pause();

        IEnumerable<IEvent> Events { get; }
    }
}