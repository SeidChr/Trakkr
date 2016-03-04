using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Trakkr.Core.Events;

namespace Trakkr.Core
{
    public interface IRepository<TId, in TPayload>
    {
        IEnumerable<TId> FindTickets(string query);
         
        bool HasTicket(TId ticketId);

        bool AddEntryToTicket(TId ticketId, IEntry<TPayload> payload);
    }
}
