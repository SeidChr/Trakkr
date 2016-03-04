using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Trakkr.Core.Events;

namespace Trakkr.Model
{
    public interface IEventCaptureSet<out TPayload>
    {
        IEvent<TPayload> Next();

        IEvent<TPayload> Pause();

        IEnumerable<IEvent<TPayload>> Events { get; }
    }
}