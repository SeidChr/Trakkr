using Trakkr.Core;

namespace Trakkr.Universal.YouTrack
{
    public class YouTrackEventPayload : IRepositoryPayload
    {
        /// <summary>
        /// To find a ticket
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Of the Found ticket.
        /// </summary>
        public string TicketId { get; set; }

        /// <summary>
        /// Event id
        /// </summary>
        public string EventId { get; set; }
    }
}