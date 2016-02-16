using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Model
{
    public interface IEventRepository
    {
        void Store(IEnumerable<IEvent> events);
        IEnumerable<IEvent> Load();
    }
}
