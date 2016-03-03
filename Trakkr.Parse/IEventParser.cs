using System;
using System.Collections.Generic;
using Trakkr.Core;

namespace Trakkr.Parse
{
    public interface IEventParser<TPayload, TIn>
    {
        IEnumerable<ITrakkrEntry<TPayload>> Parse(TIn input);
    }
}
