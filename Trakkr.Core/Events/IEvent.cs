namespace Trakkr.Core.Events
{
    public interface IEvent<out TPayload> : IBaseEvent<TPayload>
    {
        EventType Type { get; }
    }
}
