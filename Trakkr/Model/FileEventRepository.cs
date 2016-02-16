using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Model
{
    [Export("TextFile", typeof(IEventRepository))]
    public class FileEventRepository : IEventRepository
    {
        public void Store(IEnumerable<IEvent> events)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent> Load()
        {
            throw new NotImplementedException();
        }
    }
}
