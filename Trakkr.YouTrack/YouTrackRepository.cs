using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trakkr.Core;
using Trakkr.Core.Events;

namespace Trakkr.YouTrack
{
    public class YouTrackRepository : IRepository<string, IRepositoryPayload>
    {
        public IEnumerable<string> FindTickets(string query)
        {
            yield return "a";
            yield return "b";
        }

        public bool HasTicket(string ticketId)
        {
            throw new NotImplementedException();
        }

        public bool AddEntryToTicket(string ticketId, IEntry<IRepositoryPayload> payload)
        {
            throw new NotImplementedException();
        }
    }
}
