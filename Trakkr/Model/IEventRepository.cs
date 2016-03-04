using System.Collections.Generic;
using Trakkr.Core.Events;

namespace Trakkr.Model
{
    public interface IEventRepository
    {
        void Store(IEnumerable<IEvent<string>> events);

        IEnumerable<IEvent<string>> Load();
    }
}
