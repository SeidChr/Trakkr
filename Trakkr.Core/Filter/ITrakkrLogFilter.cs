using System.Collections.Generic;

namespace Trakkr.Core.Filter
{
    public interface ITrakkrLogFilter
    {
        IEnumerable<Trakkr.Core.Events.IEntry<TPayload>> Filter<TPayload>(
            IEnumerable<Trakkr.Core.Events.IEntry<TPayload>> entries);
    }
}