using System;
using System.Collections.Generic;

namespace Trakkr.Core.Filter
{
    public interface ITrakkrDayFilter
    {
        IEnumerable<Trakkr.Core.Events.IEntry<TPayload>> Filter<TPayload>(
            IEnumerable<Trakkr.Core.Events.IEntry<TPayload>> entries, 
            DateTime day);
    }
}