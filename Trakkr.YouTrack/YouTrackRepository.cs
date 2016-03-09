using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trakkr.Core;
using Trakkr.Core.Events;
using YouTrackSharp.Issues;

namespace Trakkr.YouTrack
{
    public class YouTrackRepository : IRepository<string, IRepositoryPayload>
    {
        private readonly IssueManagement issueManagement;

        public YouTrackRepository(IssueManagement issueManagement)
        {
            this.issueManagement = issueManagement;
        }

        public IEnumerable<string> FindTickets(string query)
        {
            return issueManagement
                .GetIssuesBySearch(query)
                .Cast<dynamic>()
                .Select(issue => issue.Id + ": " + issue.summary)
                .Cast<string>();
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
