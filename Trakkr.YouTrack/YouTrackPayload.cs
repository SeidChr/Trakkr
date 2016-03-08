using System.Security.Cryptography.X509Certificates;
using JsonFx.Linq;
using Trakkr.Core;

namespace Trakkr.YouTrack
{
    public class YouTrackPayload : IRepositoryPayload
    {
        public string Query { get; set; }

        public string Description { get; set; }

        public string TicketId { get; set; }

        public string WorkItemId { get; set; }

        public IRepository<string, IRepositoryPayload> Repository { get; set; }
    }
}