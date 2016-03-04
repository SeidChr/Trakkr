using System;
using System.Collections.Generic;
using Trakkr.Core;
using Trakkr.Core.Events;

namespace Trakkr.Parse
{
    public interface IEventParser<out TPayload, in TIn>
    {
        IEnumerable<IEvent<TPayload>> Parse(TIn input);
    }
}
