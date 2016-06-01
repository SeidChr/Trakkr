namespace Trakkr.Core
{
    public interface IRepositoryPayload
    {
        string Query { get; set; }

        string TicketId { get; set; }

        string EventId { get; set; }

        string Description { get; set; }
    }
}
